﻿using System;
using System.Collections.Generic;
using Irony.Parsing;
using Workbench.Core.Grammars;
using Workbench.Core.Nodes;

namespace Workbench.Core.Parsers
{
    public class DomainExpressionParser
    {
        private readonly DomainGrammar grammar = new DomainGrammar();

        /// <summary>
        /// Parse a raw domain expression.
        /// </summary>
        /// <param name="rawExpression">Raw domain expression.</param>
        /// <returns>Parse result.</returns>
        public ParseResult<DomainExpressionNode> Parse(string rawExpression)
        {
            var language = new LanguageData(grammar);
            var parser = new Parser(language);
            var parseTree = parser.Parse(rawExpression);

            return CreateResultFrom(parseTree);
        }

        private static ParseResult<DomainExpressionNode> CreateResultFrom(ParseTree parseTree)
        {
            switch (parseTree.Status)
            {
                case ParseTreeStatus.Error:
                    return new ParseResult<DomainExpressionNode>(ConvertStatusFrom(parseTree.Status),
                                                                 new List<string>());

                case ParseTreeStatus.Parsed:
                    return new ParseResult<DomainExpressionNode>(ParseStatus.Success, parseTree);

                default:
                    throw new NotImplementedException();
            }
        }

        private static ParseStatus ConvertStatusFrom(ParseTreeStatus status)
        {
            switch (status)
            {
                case ParseTreeStatus.Parsed:
                    return ParseStatus.Success;

                case ParseTreeStatus.Error:
                    return ParseStatus.Failed;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
