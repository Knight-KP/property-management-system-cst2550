using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PropertyManagementConsole.DataStructures;

namespace PropertyManagementConsole.Tests;

[TestClass]
public class BstTests
{
    [TestMethod]
    public void InsertAndSearch_FindsInsertedValues()
    {
        var bst = new BinarySearchTree<string>();
        bst.Insert(2, "B");
        bst.Insert(1, "A");
        bst.Insert(3, "C");

        Assert.AreEqual("A", bst.Search(1));
        Assert.AreEqual("B", bst.Search(2));
        Assert.AreEqual("C", bst.Search(3));
    }

    [TestMethod]
    public void Search_MissingKey_ReturnsNull()
    {
        var bst = new BinarySearchTree<string>();
        bst.Insert(10, "X");

        var result = bst.Search(99);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Insert_SameKey_OverridesValue()
    {
        var bst = new BinarySearchTree<string>();
        bst.Insert(5, "Old");
        bst.Insert(5, "New");

        Assert.AreEqual("New", bst.Search(5));
    }
}
