using System;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;

[TestFixture] class _1Test{
    private Mock<IList<int>>_list;
    private_1__1;
        
        [SetUp]publicvoidSetUp(){
        var str = default(string);
        _list = new Mock<IList<int>>();
        __1 = new _1(_list.Object, str);}[Test]publicvoidfunc(){var a = default(string);var b = default(string);var actual = __1.func(a, b);var expected = default(string);Assert.That(actual,Is.EqualTo(expected));Assert.Fail("autogenerated");}}