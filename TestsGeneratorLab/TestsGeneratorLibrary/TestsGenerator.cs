using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestsGeneratorLibrary.CodeStructures;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TestsGeneratorLibrary
{
    public class TestsGenerator : ITestsGenerator
    {
        public Dictionary<string, string> generateTests(CodeData data)
        {
            var codeDictionary = new Dictionary<string, string>();

            foreach (var classData in data.classesData)
            {
                var classTest = generateClass(classData);
                var code = SyntaxFactory.CompilationUnit()
                    .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")))
                    .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("NUnit.Framework")))
                    .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Moq")))
                    .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic")))
                    .AddMembers(classTest);


                var syntax = code.NormalizeWhitespace();

                var testCode = syntax.ToFullString();
                
              
                codeDictionary[classData.name + "Test"] = testCode;
            }

            return codeDictionary;
        }

        private ClassDeclarationSyntax generateClass(ClassData data)
        {
            var classFields = new List<FieldDeclarationSyntax>();
            VariableDeclarationSyntax variable;
            var interfaces = new Dictionary<string, string>();
            ConstructorData constructor = new ConstructorData();

            if (data.constructors.Count > 0)
            {
                constructor = getTheLargestContructor(data.constructors);
                interfaces = getInterfaces(constructor.parameters);
                foreach (var _interface in interfaces)
                {
                    variable = generateVariable("_" + _interface.Key, $"Mock<{_interface.Value}>");
                    classFields.Add(generateField(variable));
                }
            }

            var classVariable = getClassVariable(data.name);
            variable = generateVariable(classVariable, data.name);

            var field = generateField(variable);

            classFields.Add(field);

            var methods = new List<MethodDeclarationSyntax>();
            methods.Add(generateSetUpMethod(data.name,constructor));
            foreach (var methodData in data.methods)
            {
                methods.Add(generateMethod(data.name,methodData));
            }

            return SyntaxFactory.ClassDeclaration(data.name + "Test")
                .AddMembers(classFields.ToArray())
                .AddMembers(methods.ToArray())
                .AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.AttributeList().Attributes.Add(SyntaxFactory.Attribute(SyntaxFactory.ParseName("TestFixture")))));
        }


        private VariableDeclarationSyntax generateVariable(string fieldName, string typeName)
        {
            return SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName(typeName))
                .AddVariables(SyntaxFactory.VariableDeclarator(fieldName));
        }

        private FieldDeclarationSyntax generateField(VariableDeclarationSyntax variable)
        {
            return SyntaxFactory.FieldDeclaration(variable)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
        }

        private StatementSyntax generateBasesTypes(string name, string type)
        {
            return SyntaxFactory.ParseStatement(string.Format
            (
                "var {0} = default({1});",
                name,
                type
            ));
        }

        private static StatementSyntax generateCustomsTypes(string name, string constructorName, string arguments = "")
        {
            return SyntaxFactory.ParseStatement(string.Format
            (
                "{0} = new {1}{2};",
                name,
                constructorName,
                $"({arguments})"
            ));
        }

        private MethodDeclarationSyntax generateSetUpMethod(string className,ConstructorData constructorData)
        {
            List<StatementSyntax> root = new List<StatementSyntax>();

            var baseTypeVaribles = getBaseType(constructorData.parameters);
            foreach (var varible in baseTypeVaribles)
            {
                root.Add(generateBasesTypes(varible.Key, varible.Value));
            }

            var interfaces = getInterfaces(constructorData.parameters);
            foreach (var _interface in interfaces)
            {
                root.Add(generateCustomsTypes("_" + _interface.Key, $"Mock<{_interface.Value}>", ""));
            }

            root.Add(generateCustomsTypes(getClassVariable(className), className, convertParametersToStr(constructorData.parameters)));
            return SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), "SetUp")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.AttributeList().Attributes.Add(SyntaxFactory.Attribute(SyntaxFactory.ParseName("SetUp")))))
                .WithBody(SyntaxFactory.Block(root)); 
        }

        private MethodDeclarationSyntax generateMethod(string classVarible,MethodData methodData)
        {
            List<StatementSyntax> body = new List<StatementSyntax>();
            generateArrange(body, methodData.parameters);
            generateAct(classVarible,body, methodData);
            if (methodData.returnType != "void")
            {
                generateAssert(body, methodData.returnType);
            }

            body.Add(createFailExpression());

            return SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), methodData.name + "Test")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.AttributeList().Attributes.Add(SyntaxFactory.Attribute(SyntaxFactory.ParseName("Test")))))
                .WithBody(SyntaxFactory.Block(body)); ;
        }

        private void generateArrange(List<StatementSyntax> body, Dictionary<string, string> parameters)
        {
            var types = getBaseType(parameters);
            foreach (var type in types)
            {
                body.Add(generateBasesTypes(type.Key, type.Value));
            }
        }


        private void generateAct(string classVariable,List<StatementSyntax> body, MethodData methodData)
        {
            if (methodData.returnType != "void")
            {
                body.Add(generateFunctionCall("actual", getClassVariable(classVariable) + "." + methodData.name, convertParametersToStr(methodData.parameters)));
            }
            else
            {
                body.Add(generateVoidFunctionCall(getClassVariable(classVariable) + "." + methodData.name, convertParametersToStr(methodData.parameters)));
            }
        }

        private void generateAssert(List<StatementSyntax> body, string returnType)
        {
            body.Add(generateBasesTypes("expected", returnType));
            var invocationExpression = generateExpression("Assert", "That");

            var secondPart = generateExpression("Is", "EqualTo").WithArgumentList(ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(
                new SyntaxNodeOrToken[] {
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("expected"))})));

            var argList = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(
                new SyntaxNodeOrToken[] {
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("actual")),
                    SyntaxFactory.Token(SyntaxKind.CommaToken),
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName(secondPart.ToString()))}));

            var statement = ExpressionStatement(invocationExpression.WithArgumentList(argList));
            body.Add(statement);
        }


        private InvocationExpressionSyntax generateExpression(string id1, string id2)
        {
            return SyntaxFactory.InvocationExpression(
                       SyntaxFactory.MemberAccessExpression(
                           SyntaxKind.SimpleMemberAccessExpression,
                           SyntaxFactory.IdentifierName(id1),
                           SyntaxFactory.IdentifierName(id2)));
        }

        private StatementSyntax generateFunctionCall(string varibleName, string functionName, string arguments = "")
        {
            return SyntaxFactory.ParseStatement(string.Format
            (
                "var {0} = {1}{2};",
                varibleName,
                functionName,
                $"({arguments})"
            ));
        }

        private StatementSyntax generateVoidFunctionCall(string functionName, string arguments = "")
        {
            return SyntaxFactory.ParseStatement(string.Format
            (
                "{0}{1};",
                functionName,
                $"({arguments})"
            ));
        }


        private string getClassVariable(string className)
        {
            return "_" + className[0].ToString().ToLower() + className.Remove(0, 1);
        }

        private ConstructorData getTheLargestContructor(List<ConstructorData> constructors)
        {
            var bestConstructor = constructors[0];
            foreach (var constructor in constructors)
            {
                if (bestConstructor.parameters.Count < constructor.parameters.Count)
                {
                    bestConstructor = constructor;
                }
            }

            return bestConstructor;
        }


        private Dictionary<string, string> getInterfaces(Dictionary<string, string> parameters)
        {
            var result = new Dictionary<string, string>();

            if (parameters == null) { return result; }

            foreach (var parameter in parameters)
            {
                if (parameter.Value[0] == 'I')
                {
                    result[parameter.Key] = parameter.Value;
                }
            }

            return result;
        }

        private Dictionary<string, string> getBaseType(Dictionary<string, string> parameters)
        {
            var res = new Dictionary<string, string>();

            if (parameters == null) { return res; } 

            foreach (var parameter in parameters)
            {
                if (parameter.Value[0] != 'I')
                {
                    res.Add(parameter.Key, parameter.Value);
                }
            }

            return res;
        }

        private string convertParametersToStr(Dictionary<string, string> parameters)
        {
            var str = "";

            if (parameters == null) { return str; }

            foreach (var param in parameters)
            {
                str += param.Value[0] == 'I' ? $"_{param.Key}.Object" : $"{param.Key}";
                str += ", ";
            }

            return str.Length > 0 ? str.Remove(str.Length - 2, 2) : "";
        }


        private ExpressionStatementSyntax createFailExpression()
        {
            return SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("Assert"),
                            SyntaxFactory.IdentifierName("Fail")))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal("autogenerated")))))));
        }
    }
}
