﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.Compilation.Parser;
using DotVVM.Framework.Compilation.Parser.Binding.Parser;
using DotVVM.Framework.Compilation.Parser.Binding.Tokenizer;
using DotVVM.Framework.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BindingParser = DotVVM.Framework.Compilation.Parser.Binding.Parser.BindingParser;

namespace DotVVM.Framework.Tests.Parser.Binding
{
    [TestClass]
    public class BindingParserTests
    {

        [TestMethod]
        public void BindingParser_TrueLiteral_Valid()
        {
            var result = Parse("true");

            Assert.IsInstanceOfType(result, typeof(LiteralExpressionBindingParserNode));
            Assert.AreEqual(true, ((LiteralExpressionBindingParserNode)result).Value);
        }

        [TestMethod]
        public void BindingParser_FalseLiteral_WhiteSpaceOnEnd_Valid()
        {
            var result = Parse("false  \t ");

            Assert.IsInstanceOfType(result, typeof(LiteralExpressionBindingParserNode));
            Assert.AreEqual(false, ((LiteralExpressionBindingParserNode)result).Value);
        }

        [TestMethod]
        public void BindingParser_NullLiteral_WhiteSpaceOnStart_Valid()
        {
            var result = Parse(" null");

            Assert.IsInstanceOfType(result, typeof(LiteralExpressionBindingParserNode));
            Assert.AreEqual(null, ((LiteralExpressionBindingParserNode)result).Value);
        }

        [TestMethod]
        public void BindingParser_SimpleProperty_Arithmetics_Valid()
        {
            var result = Parse("a +b");

            var binaryOperator = (BinaryOperatorBindingParserNode)result;
            Assert.AreEqual("a", ((IdentifierNameBindingParserNode)binaryOperator.FirstExpression).Name);
            Assert.AreEqual("b", ((IdentifierNameBindingParserNode)binaryOperator.SecondExpression).Name);
            Assert.AreEqual(BindingTokenType.AddOperator, binaryOperator.Operator);
        }

        [TestMethod]
        public void BindingParser_MemberAccess_Arithmetics_Valid()
        {
            var result = Parse("a.c - b");

            var binaryOperator = (BinaryOperatorBindingParserNode)result;
            var first = (MemberAccessBindingParserNode)binaryOperator.FirstExpression;
            Assert.AreEqual("a", ((IdentifierNameBindingParserNode)first.TargetExpression).Name);
            Assert.AreEqual("c", first.MemberNameExpression.Name);
            Assert.AreEqual("b", ((IdentifierNameBindingParserNode)binaryOperator.SecondExpression).Name);
            Assert.AreEqual(BindingTokenType.SubtractOperator, binaryOperator.Operator);
        }

        [TestMethod]
        public void BindingParser_NestedMemberAccess_Number_ArithmeticsOperatorPrecendence_Valid()
        {
            var result = Parse("a.c.d * b + 3.14");

            var binaryOperator = (BinaryOperatorBindingParserNode)result;
            Assert.AreEqual(BindingTokenType.AddOperator, binaryOperator.Operator);

            var first = (BinaryOperatorBindingParserNode)binaryOperator.FirstExpression;
            Assert.AreEqual(BindingTokenType.MultiplyOperator, first.Operator);
            var acd = (MemberAccessBindingParserNode)first.FirstExpression;
            var ac = (MemberAccessBindingParserNode)acd.TargetExpression;
            Assert.AreEqual("a", ((IdentifierNameBindingParserNode)ac.TargetExpression).Name);
            Assert.AreEqual("c", ac.MemberNameExpression.Name);
            Assert.AreEqual("d", acd.MemberNameExpression.Name);
            Assert.AreEqual("b", ((IdentifierNameBindingParserNode)first.SecondExpression).Name);

            var second = (LiteralExpressionBindingParserNode)binaryOperator.SecondExpression;
            Assert.AreEqual(3.14, second.Value);
        }

        [TestMethod]
        public void BindingParser_ArithmeticOperatorPrecedence_Parenthesis_Valid()
        {
            var result = Parse("a + b * c - d / (e + 2)");

            var root = (BinaryOperatorBindingParserNode)result;
            Assert.AreEqual(BindingTokenType.AddOperator, root.Operator);

            var a = (IdentifierNameBindingParserNode)root.FirstExpression;
            Assert.AreEqual("a", a.Name);

            var subtract = (BinaryOperatorBindingParserNode)root.SecondExpression;
            Assert.AreEqual(BindingTokenType.SubtractOperator, subtract.Operator);

            var multiply = (BinaryOperatorBindingParserNode)subtract.FirstExpression;
            Assert.AreEqual(BindingTokenType.MultiplyOperator, multiply.Operator);
            Assert.AreEqual("b", ((IdentifierNameBindingParserNode)multiply.FirstExpression).Name);
            Assert.AreEqual("c", ((IdentifierNameBindingParserNode)multiply.SecondExpression).Name);

            var divide = (BinaryOperatorBindingParserNode)subtract.SecondExpression;
            Assert.AreEqual(BindingTokenType.DivideOperator, divide.Operator);
            Assert.AreEqual("d", ((IdentifierNameBindingParserNode)divide.FirstExpression).Name);

            var parenthesis = (ParenthesizedExpressionBindingParserNode)divide.SecondExpression;
            var addition2 = (BinaryOperatorBindingParserNode)parenthesis.InnerExpression;
            Assert.AreEqual(BindingTokenType.AddOperator, addition2.Operator);
            Assert.AreEqual("e", ((IdentifierNameBindingParserNode)addition2.FirstExpression).Name);
            Assert.AreEqual(2, ((LiteralExpressionBindingParserNode)addition2.SecondExpression).Value);
        }

        [TestMethod]
        public void BindingParser_ArithmeticOperatorChain_Valid()
        {
            var result = Parse("a + b + c");

            var root = (BinaryOperatorBindingParserNode)result;
            Assert.AreEqual(BindingTokenType.AddOperator, root.Operator);

            var a = (IdentifierNameBindingParserNode)root.FirstExpression;
            Assert.AreEqual("a", a.Name);

            var add = (BinaryOperatorBindingParserNode)root.SecondExpression;
            Assert.AreEqual(BindingTokenType.AddOperator, add.Operator);

            Assert.AreEqual("b", ((IdentifierNameBindingParserNode)add.FirstExpression).Name);
            Assert.AreEqual("c", ((IdentifierNameBindingParserNode)add.SecondExpression).Name);
        }


        [TestMethod]
        public void BindingParser_MemberAccess_ArrayIndexer_Chain_Valid()
        {
            var result = Parse("a[b + -1](c).d[e ?? f]");

            var root = (ArrayAccessBindingParserNode)result;

            var ef = (BinaryOperatorBindingParserNode)root.ArrayIndexExpression;
            Assert.AreEqual(BindingTokenType.NullCoalescingOperator, ef.Operator);
            Assert.AreEqual("e", ((IdentifierNameBindingParserNode)ef.FirstExpression).Name);
            Assert.AreEqual("f", ((IdentifierNameBindingParserNode)ef.SecondExpression).Name);

            var d = (MemberAccessBindingParserNode)root.TargetExpression;
            Assert.AreEqual("d", d.MemberNameExpression.Name);

            var functionCall = (FunctionCallBindingParserNode)d.TargetExpression;
            Assert.AreEqual(1, functionCall.ArgumentExpressions.Count);
            Assert.AreEqual("c", ((IdentifierNameBindingParserNode)functionCall.ArgumentExpressions[0]).Name);

            var firstArray = (ArrayAccessBindingParserNode)functionCall.TargetExpression;
            var add = (BinaryOperatorBindingParserNode)firstArray.ArrayIndexExpression;
            Assert.AreEqual(BindingTokenType.AddOperator, add.Operator);
            Assert.AreEqual("b", ((IdentifierNameBindingParserNode)add.FirstExpression).Name);
            Assert.AreEqual(1, ((LiteralExpressionBindingParserNode)((UnaryOperatorBindingParserNode)add.SecondExpression).InnerExpression).Value);
            Assert.AreEqual(BindingTokenType.SubtractOperator, ((UnaryOperatorBindingParserNode)add.SecondExpression).Operator);

            Assert.AreEqual("a", ((IdentifierNameBindingParserNode)firstArray.TargetExpression).Name);
        }

        [TestMethod]
        public void BindingParser_StringLiteral_Valid()
        {
            var result = Parse("\"help\\\"help\"");
            Assert.AreEqual("help\"help", ((LiteralExpressionBindingParserNode)result).Value);
        }

        [TestMethod]
        public void BindingParser_StringLiteral_SingleQuotes_Valid()
        {
            var result = Parse("'help\\nhelp'");
            Assert.AreEqual("help\nhelp", ((LiteralExpressionBindingParserNode)result).Value);
        }

        [TestMethod]
        public void BindingParser_ConditionalOperator_Valid()
        {
            var result = Parse("a ? !b : c");
            var condition = (ConditionalExpressionBindingParserNode)result;
            Assert.AreEqual("a", ((IdentifierNameBindingParserNode)condition.ConditionExpression).Name);
            Assert.AreEqual("b", ((IdentifierNameBindingParserNode)((UnaryOperatorBindingParserNode)condition.TrueExpression).InnerExpression).Name);
            Assert.AreEqual(BindingTokenType.NotOperator, ((UnaryOperatorBindingParserNode)condition.TrueExpression).Operator);
            Assert.AreEqual("c", ((IdentifierNameBindingParserNode)condition.FalseExpression).Name);
        }

        [TestMethod]
        public void BindingParser_Empty_Invalid()
        {
            var result = Parse("");
            Assert.IsTrue(((IdentifierNameBindingParserNode)result).HasNodeErrors);
        }

        [TestMethod]
        public void BindingParser_Whitespace_Invalid()
        {
            var result = Parse(" ");
            Assert.IsTrue(((IdentifierNameBindingParserNode)result).HasNodeErrors);
            Assert.AreEqual(0, result.StartPosition);
            Assert.AreEqual(1, result.Length);
        }

        [TestMethod]
        public void BindingParser_Incomplete_Expression()
        {
            var result = Parse(" (a +");
            Assert.IsTrue(((ParenthesizedExpressionBindingParserNode)result).HasNodeErrors);
            Assert.AreEqual(0, result.StartPosition);
            Assert.AreEqual(5, result.Length);

            var inner = (BinaryOperatorBindingParserNode)((ParenthesizedExpressionBindingParserNode)result).InnerExpression;
            Assert.AreEqual(BindingTokenType.AddOperator, inner.Operator);
            Assert.AreEqual("a", ((IdentifierNameBindingParserNode)inner.FirstExpression).Name);
            Assert.AreEqual("", ((IdentifierNameBindingParserNode)inner.SecondExpression).Name);
            Assert.IsTrue(inner.SecondExpression.HasNodeErrors);
            Assert.AreEqual(2, inner.FirstExpression.Length);
            Assert.AreEqual(0, inner.SecondExpression.Length);
        }

        [TestMethod]
        public void BindingParser_IntLiteral_Valid()
        {
            var result = (LiteralExpressionBindingParserNode)Parse("12");
            Assert.IsInstanceOfType(result.Value, typeof(int));
            Assert.AreEqual(result.Value, 12);
        }

        [TestMethod]
        public void BindingParser_DoubleLiteral_Valid()
        {
            var result = (LiteralExpressionBindingParserNode)Parse("12.45");
            Assert.IsInstanceOfType(result.Value, typeof(double));
            Assert.AreEqual(result.Value, 12.45);
        }

        [TestMethod]
        public void BindingParser_FloatLiteral_Valid()
        {
            var result = (LiteralExpressionBindingParserNode)Parse("42f");
            Assert.IsInstanceOfType(result.Value, typeof(float));
            Assert.AreEqual(result.Value, 42f);
        }

        [TestMethod]
        public void BindingParser_LongLiteral_Valid()
        {
            var result = (LiteralExpressionBindingParserNode)Parse(long.MaxValue.ToString());
            Assert.IsInstanceOfType(result.Value, typeof(long));
            Assert.AreEqual(result.Value, long.MaxValue);
        }

        [TestMethod]
        public void BindingParser_LongForcedLiteral_Valid()
        {
            var result = (LiteralExpressionBindingParserNode)Parse("42L");
            Assert.IsInstanceOfType(result.Value, typeof(long));
            Assert.AreEqual(result.Value, 42L);
        }

        [TestMethod]
        public void BindingParser_MethodInvokeOnValue_Valid()
        {
            var result = (FunctionCallBindingParserNode)Parse("42.ToString()");
            var memberAccess = (MemberAccessBindingParserNode)result.TargetExpression;
            Assert.AreEqual(memberAccess.MemberNameExpression.Name, "ToString");
            Assert.AreEqual(((LiteralExpressionBindingParserNode)memberAccess.TargetExpression).Value, 42);
            Assert.AreEqual(result.ArgumentExpressions.Count, 0);
        }

        [TestMethod]
        public void BindingParser_AssignOperator_Valid()
        {
            var result = (BinaryOperatorBindingParserNode)Parse("a = b");
            Assert.AreEqual(BindingTokenType.AssignOperator, result.Operator);

            var first = (IdentifierNameBindingParserNode)result.FirstExpression;
            Assert.AreEqual("a", first.Name);

            var second = (IdentifierNameBindingParserNode)result.SecondExpression;
            Assert.AreEqual("b", second.Name);
        }

        [TestMethod]
        public void BindingParser_AssignOperator_Incomplete()
        {
            var result = (BinaryOperatorBindingParserNode)Parse("a = ");
            Assert.AreEqual(BindingTokenType.AssignOperator, result.Operator);

            var first = (IdentifierNameBindingParserNode)result.FirstExpression;
            Assert.AreEqual("a", first.Name);

            var second = (IdentifierNameBindingParserNode)result.SecondExpression;
            Assert.IsTrue(second.HasNodeErrors);
        }

        [TestMethod]
        public void BindingParser_AssignOperator_Incomplete1()
        {
            var result = (BinaryOperatorBindingParserNode)Parse("=");
            Assert.AreEqual(BindingTokenType.AssignOperator, result.Operator);

            var first = (IdentifierNameBindingParserNode)result.FirstExpression;
            Assert.IsTrue(first.HasNodeErrors);

            var second = (IdentifierNameBindingParserNode)result.SecondExpression;
            Assert.IsTrue(second.HasNodeErrors);
        }

        [TestMethod]
        public void BindingParser_PlusAssign_Valid()
        {
            var parser = SetupParser("_root.MyCoolProperty += 3");
            var node = parser.ReadExpression();

            Assert.IsTrue(parser.OnEnd());
            Assert.IsTrue(node is BinaryOperatorBindingParserNode);

            var binOpNode = node as BinaryOperatorBindingParserNode;

            Assert.IsTrue(binOpNode.FirstExpression is MemberAccessBindingParserNode);
            Assert.IsTrue(binOpNode.SecondExpression is LiteralExpressionBindingParserNode);
            Assert.IsTrue(binOpNode.Operator == BindingTokenType.UnsupportedOperator);
            Assert.IsTrue(binOpNode.NodeErrors[0] == "Unsupported operator: +=");
        }

        [TestMethod]
        public void BindingParser_MultipleUnsuportedBinaryOperators_Valid()
        {
            var parser = SetupParser("_root.MyCoolProperty += _this.Number1 + Number2^_parent0.Exponent * Multiplikator");
            var node = parser.ReadExpression();

            Assert.IsTrue(parser.OnEnd());
            Assert.IsInstanceOfType(node, typeof(BinaryOperatorBindingParserNode));

            var plusAssignNode = node as BinaryOperatorBindingParserNode;

            CheckBinaryOperatorNodeType<MemberAccessBindingParserNode, BinaryOperatorBindingParserNode>(plusAssignNode, BindingTokenType.UnsupportedOperator);

            var caretNode = plusAssignNode.SecondExpression as BinaryOperatorBindingParserNode;

            CheckBinaryOperatorNodeType<BinaryOperatorBindingParserNode, BinaryOperatorBindingParserNode>(caretNode, BindingTokenType.UnsupportedOperator);

            var plusNode = caretNode.FirstExpression as BinaryOperatorBindingParserNode;

            CheckBinaryOperatorNodeType<MemberAccessBindingParserNode, IdentifierNameBindingParserNode>(plusNode, BindingTokenType.AddOperator);

            var multiplyNode = caretNode.SecondExpression as BinaryOperatorBindingParserNode;

            CheckBinaryOperatorNodeType<MemberAccessBindingParserNode, IdentifierNameBindingParserNode>(multiplyNode, BindingTokenType.MultiplyOperator);

            Assert.IsTrue(caretNode.NodeErrors.Any());
            Assert.IsTrue(plusAssignNode.NodeErrors.Any());
        }

        [TestMethod]
        public void BindingParser_UnsuportedUnaryOperators_Valid()
        {
            var parser = SetupParser("MyCoolProperty = ^&Number1 + ^&Number2 * ^&Number3");
            var node = parser.ReadExpression();

            Assert.IsTrue(parser.OnEnd());
            Assert.IsInstanceOfType(node, typeof(BinaryOperatorBindingParserNode));

            var assignNode = node as BinaryOperatorBindingParserNode;

            CheckBinaryOperatorNodeType<IdentifierNameBindingParserNode, BinaryOperatorBindingParserNode>(assignNode, BindingTokenType.AssignOperator);

            var plusNode = assignNode.SecondExpression as BinaryOperatorBindingParserNode;

            CheckBinaryOperatorNodeType<UnaryOperatorBindingParserNode, BinaryOperatorBindingParserNode>(plusNode, BindingTokenType.AddOperator);

            var multiplyNode = plusNode.SecondExpression as BinaryOperatorBindingParserNode;

            CheckBinaryOperatorNodeType<UnaryOperatorBindingParserNode, UnaryOperatorBindingParserNode>(multiplyNode, BindingTokenType.MultiplyOperator);

            var Number1Unary = plusNode.FirstExpression as UnaryOperatorBindingParserNode;
            var Number2Unary = multiplyNode.FirstExpression as UnaryOperatorBindingParserNode;
            var Number3Unary = multiplyNode.SecondExpression as UnaryOperatorBindingParserNode;

            CheckUnaryOperatorNodeType<IdentifierNameBindingParserNode>(Number1Unary, BindingTokenType.UnsupportedOperator);
            CheckUnaryOperatorNodeType<IdentifierNameBindingParserNode>(Number2Unary, BindingTokenType.UnsupportedOperator);
            CheckUnaryOperatorNodeType<IdentifierNameBindingParserNode>(Number3Unary, BindingTokenType.UnsupportedOperator);

            Assert.IsTrue(Number1Unary.NodeErrors.Any());
            Assert.IsTrue(Number2Unary.NodeErrors.Any());
            Assert.IsTrue(Number3Unary.NodeErrors.Any());
        }

        [TestMethod]
        public void BindingParser_BinaryAndUnaryUnsuportedOperators_Valid()
        {
            var parser = SetupParser("MyCoolProperty += ^& Number1");
            var node = parser.ReadExpression();

            Assert.IsTrue(parser.OnEnd());
            Assert.IsInstanceOfType(node, typeof(BinaryOperatorBindingParserNode));

            var plusAssignNode = node as BinaryOperatorBindingParserNode;

            CheckBinaryOperatorNodeType<IdentifierNameBindingParserNode, UnaryOperatorBindingParserNode>(plusAssignNode, BindingTokenType.UnsupportedOperator);

            var Number1Unary = plusAssignNode.SecondExpression as UnaryOperatorBindingParserNode;

            CheckUnaryOperatorNodeType<IdentifierNameBindingParserNode>(Number1Unary, BindingTokenType.UnsupportedOperator);

            Assert.IsTrue(Number1Unary.NodeErrors.Any());
        }

        [TestMethod]
        public void BindingParser_MultiExpression_MemberAccessAndExplicitStrings()
        {
            var parser = SetupParser("_root.MyCoolProperty 'something' \"something else\"");
            var node = parser.ReadMultiExpression();

            Assert.IsTrue(parser.OnEnd());
            Assert.IsTrue(node is MultiExpressionBindingParserNode);

            var multiExpresionNode = node as MultiExpressionBindingParserNode;

            Assert.IsTrue(multiExpresionNode.Expressions.Count == 3);

            Assert.IsTrue(multiExpresionNode.Expressions[0] is MemberAccessBindingParserNode);
            Assert.IsTrue(multiExpresionNode.Expressions[1] is LiteralExpressionBindingParserNode);
            Assert.IsTrue(multiExpresionNode.Expressions[2] is LiteralExpressionBindingParserNode);
        }

        [TestMethod]
        public void BindingParser_MultiExpression_SuperUnfriendlyContent()
        {
            var parser = SetupParser(@"
                    IsCanceled ? '}"" ValueBinding=""{value: Currency}"" HeaderText=""Currency"" />
           
                <dot:GridViewTemplateColumn HeaderText="""" >
                    <ContentTemplate>
                        <dot:RouteLink RouteName=""AdminOrderDetail"" Param -Id=""{ value: Id}"" >
                            <bs:GlyphIcon Icon=""Search"" />
                        </dot:RouteLink>
                    </ContentTemplate>
                </dot:GridViewTemplateColumn>
                <dot:GridViewTemplateColumn HeaderText="""" >
                    <ContentTemplate>
                        <dot:RouteLink RouteName=""OrderPaymentReceipt"" Visible =""{ value:  PaidDate != null}"" Param -OrderId=""{ value: Id}"" >
                            <bs:GlyphIcon Icon=""Download_alt"" />
                        </dot:RouteLink>
                    </ContentTemplate>
                </dot:GridViewTemplateColumn>
            </Columns>
            <EmptyDataTemplate>
                There are no orders to show. &nbsp; :'(
            </EmptyDataTemplate>
        </bs:GridView>
        <dot:DataPager class=""pagination"" DataSet =""{ value: Orders}"" />
    </bs:Container>
</dot:Content>
");
            var node = parser.ReadMultiExpression();

            Assert.IsTrue(parser.OnEnd());
            Assert.IsTrue(node is MultiExpressionBindingParserNode);

            var multiExpresionNode = node as MultiExpressionBindingParserNode;

            Assert.IsTrue(multiExpresionNode.Expressions.Count == 12);
        }

        [TestMethod]
        public void BindingParser_MultiExpression_MemberAccessUnsupportedOperatorAndExplicitStrings()
        {
            var parser = SetupParser("_root.MyCoolProperty += 'something' \"something else\"");
            var node = parser.ReadMultiExpression();

            Assert.IsTrue(parser.OnEnd());
            Assert.IsTrue(node is MultiExpressionBindingParserNode);

            var multiExpresionNode = node as MultiExpressionBindingParserNode;

            Assert.IsTrue(multiExpresionNode.Expressions.Count == 2);

            Assert.IsTrue(multiExpresionNode.Expressions[0] is BinaryOperatorBindingParserNode);
            Assert.IsTrue(multiExpresionNode.Expressions[1] is LiteralExpressionBindingParserNode);
        }

        [TestMethod]
        public void BindingParser_NodeTokenCorrectness_UnsupportedOperators()
        {
            var parser = SetupParser("_this.MyCoolProperty +=  _control.ClientId &^ _root += Comments");
            var node = parser.ReadExpression();

            Assert.IsTrue(parser.OnEnd());
            Assert.IsTrue(node is BinaryOperatorBindingParserNode);

            var plusEqualsExp1 = node as BinaryOperatorBindingParserNode;

            var myCoolPropertyExp = plusEqualsExp1.FirstExpression as MemberAccessBindingParserNode;
            var andCarretExp = plusEqualsExp1.SecondExpression as BinaryOperatorBindingParserNode;

            var clientIdExp = andCarretExp.FirstExpression as MemberAccessBindingParserNode;
            var plusEqualsExp2 = andCarretExp.SecondExpression as BinaryOperatorBindingParserNode;

            var rootExp = plusEqualsExp2.FirstExpression as IdentifierNameBindingParserNode;
            var commentsExp = plusEqualsExp2.SecondExpression as IdentifierNameBindingParserNode;

            //_this.MyCoolProperty +=  _control.ClientId &^ _root += Comments//
            CheckTokenTypes(plusEqualsExp1.Tokens, new BindingTokenType[] {
                BindingTokenType.Identifier,
                BindingTokenType.Dot,
                BindingTokenType.Identifier,
                BindingTokenType.WhiteSpace,
                BindingTokenType.UnsupportedOperator,
                BindingTokenType.WhiteSpace,
                BindingTokenType.Identifier,
                BindingTokenType.Dot,
                BindingTokenType.Identifier,
                BindingTokenType.WhiteSpace,
                BindingTokenType.UnsupportedOperator,
                BindingTokenType.WhiteSpace,
                BindingTokenType.Identifier,
                BindingTokenType.WhiteSpace,
                BindingTokenType.UnsupportedOperator,
                BindingTokenType.WhiteSpace,
                BindingTokenType.Identifier
            });

            //_this.MyCoolProperty //
            CheckTokenTypes(myCoolPropertyExp.Tokens, new BindingTokenType[] {
                BindingTokenType.Identifier,
                BindingTokenType.Dot,
                BindingTokenType.Identifier,
                BindingTokenType.WhiteSpace
            });

            //  _control.ClientId &^ _root += Comments//
            CheckTokenTypes(andCarretExp.Tokens, new BindingTokenType[] {
                BindingTokenType.WhiteSpace,
                BindingTokenType.Identifier,
                BindingTokenType.Dot,
                BindingTokenType.Identifier,
                BindingTokenType.WhiteSpace,
                BindingTokenType.UnsupportedOperator,
                BindingTokenType.WhiteSpace,
                BindingTokenType.Identifier,
                BindingTokenType.WhiteSpace,
                BindingTokenType.UnsupportedOperator,
                BindingTokenType.WhiteSpace,
                BindingTokenType.Identifier
            });

            //  _control.ClientId //
            CheckTokenTypes(clientIdExp.Tokens, new BindingTokenType[] {
                BindingTokenType.WhiteSpace,
                BindingTokenType.Identifier,
                BindingTokenType.Dot,
                BindingTokenType.Identifier,
                BindingTokenType.WhiteSpace
            });

            // _root += Comments//
            CheckTokenTypes(plusEqualsExp2.Tokens, new BindingTokenType[] {
                BindingTokenType.WhiteSpace,
                BindingTokenType.Identifier,
                BindingTokenType.WhiteSpace,
                BindingTokenType.UnsupportedOperator,
                BindingTokenType.WhiteSpace,
                BindingTokenType.Identifier
            });

            // _root //
            CheckTokenTypes(rootExp.Tokens, new BindingTokenType[] {
                BindingTokenType.WhiteSpace,
                BindingTokenType.Identifier,
                BindingTokenType.WhiteSpace,
            });

            // Comments//
            CheckTokenTypes(commentsExp.Tokens, new BindingTokenType[] {
                BindingTokenType.WhiteSpace,
                BindingTokenType.Identifier
            });
        }

        private static BindingParserNode Parse(string expression)
        {
            BindingParser parser = SetupParser(expression);
            return parser.ReadExpression();
        }

        private static BindingParser SetupParser(string expression)
        {
            var tokenizer = new BindingTokenizer();
            tokenizer.Tokenize(expression);
            var parser = new BindingParser();
            parser.Tokens = tokenizer.Tokens;
            return parser;
        }

        private static void CheckTokenTypes(IEnumerable<BindingToken> bindingTokens, IEnumerable<BindingTokenType> expectedTokenTypes)
        {
            var actualTypes = bindingTokens.Select(t => t.Type);

            Assert.IsTrue(Enumerable.SequenceEqual(actualTypes, expectedTokenTypes));
        }

        private static void CheckUnaryOperatorNodeType<TInnerExpression>(UnaryOperatorBindingParserNode node, BindingTokenType operatorType)
           where TInnerExpression : BindingParserNode
        {
            Assert.IsTrue(node.Operator == operatorType);
            Assert.IsTrue(node.InnerExpression is TInnerExpression);
        }

        private static void CheckBinaryOperatorNodeType<TLeft, TRight>(BinaryOperatorBindingParserNode node, BindingTokenType operatorType)
            where TLeft : BindingParserNode
            where TRight : BindingParserNode
        {
            Assert.IsTrue(node.Operator == operatorType);
            Assert.IsTrue(node.FirstExpression is TLeft);
            Assert.IsTrue(node.SecondExpression is TRight);
        }
    }
}
