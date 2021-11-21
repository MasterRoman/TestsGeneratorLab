using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using TestsGeneratorLibrary.CodeStructures;

namespace TestsGeneratorLibrary
{
    public class Parser : IParser
    {
        public CodeData parse(string code)
        {
            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var classes = new List<ClassData>();
            foreach (ClassDeclarationSyntax classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                classes.Add(parseClass(classDeclaration));
            }
        
            return new CodeData(classes);
        }

        private ClassData parseClass(ClassDeclarationSyntax classDeclaration)
        {
            var constructors = new List<ConstructorData>();
            foreach (var constructor in classDeclaration.DescendantNodes().OfType<ConstructorDeclarationSyntax>().Where((constructorDeclaration) => constructorDeclaration.Modifiers.Any((modifier) => modifier.IsKind(SyntaxKind.PublicKeyword))))
            {
                constructors.Add(parseConstructor(constructor));
            }

            var methods = new List<MethodData>();
            foreach (var method in classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>().Where((methodDeclaration) => methodDeclaration.Modifiers.Any((modifier) => modifier.IsKind(SyntaxKind.PublicKeyword))))
            {
                methods.Add(parseMethod(method));
            }

            var className = classDeclaration.Identifier.ValueText;


            return new ClassData(className, constructors,methods);
        }


        private ConstructorData parseConstructor(ConstructorDeclarationSyntax constructor)
        {
            var parameters = new Dictionary<string, string>();
            foreach (var parameter in constructor.ParameterList.Parameters)
            {
                parameters.Add(parameter.Identifier.Text, parameter.Type.ToString());
            }

            var constructorName = constructor.Identifier.ValueText;

            return new ConstructorData(constructorName, parameters);
        }

        private MethodData parseMethod(MethodDeclarationSyntax method)
        {
            var parameters = new Dictionary<string, string>();
            foreach (var parameter in method.ParameterList.Parameters)
            {
                parameters.Add(parameter.Identifier.Text, parameter.Type.ToString());
            }

            var methodName = method.Identifier.ValueText;
            var returnTypeStr = method.ReturnType.ToString();

            return new MethodData(methodName, parameters, returnTypeStr);
        }

    }
}
