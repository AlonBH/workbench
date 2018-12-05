﻿using System;
using System.Xml;
using Workbench.Core.Models;

namespace Workbench.Services
{
    internal class XmlConstraintWriter
    {
        private readonly XmlDocument _document;
        private readonly ModelModel _model;

        internal XmlConstraintWriter(XmlDocument theDocument, ModelModel theModel)
        {
            _document = theDocument;
            _model = theModel;
        }

        internal void Write(XmlElement modelRoot)
        {
            var constraintsRoot = _document.CreateElement("constraints");
            foreach (var aConstraint in _model.Constraints)
            {
                switch (aConstraint)
                {
                    case AllDifferentConstraintModel allDifferentConstraint:
                        WriteConstraint(allDifferentConstraint, constraintsRoot);
                        break;

                    case ExpressionConstraintModel expressionConstraint:
                        WriteConstraint(expressionConstraint, constraintsRoot);
                        break;

                    default:
                        throw new NotImplementedException("Unknown constraint type.");
                }
            }
            modelRoot.AppendChild(constraintsRoot);
        }

        private void WriteConstraint(ExpressionConstraintModel expressionConstraint, XmlElement constraintsRoot)
        {
            var expressionConstraintElement = _document.CreateElement("expression-constraint");
            var idAttribute = _document.CreateAttribute("id");
            idAttribute.Value = Convert.ToString(expressionConstraint.Id);
            expressionConstraintElement.Attributes.Append(idAttribute);
            var nameAttribute = _document.CreateAttribute("name");
            nameAttribute.Value = expressionConstraint.Name;
            expressionConstraintElement.Attributes.Append(nameAttribute);
            var expressionElement = _document.CreateElement("expression");
            var encodedExpressionNode = _document.CreateCDataSection(expressionConstraint.Expression.Text);
            expressionElement.AppendChild(encodedExpressionNode);
            expressionConstraintElement.AppendChild(expressionElement);
            constraintsRoot.AppendChild(expressionConstraintElement);
        }

        private void WriteConstraint(AllDifferentConstraintModel allDifferentConstraint, XmlElement constraintsRoot)
        {
            var allDifferentElement = _document.CreateElement("all-different-constraint");
            var idAttribute = _document.CreateAttribute("id");
            idAttribute.Value = Convert.ToString(allDifferentConstraint.Id);
            allDifferentElement.Attributes.Append(idAttribute);
            var nameAttribute = _document.CreateAttribute("name");
            nameAttribute.Value = allDifferentConstraint.Name;
            allDifferentElement.Attributes.Append(nameAttribute);
            var expressionElement = _document.CreateElement("expression");
            var encodedExpressionNode = _document.CreateCDataSection(allDifferentConstraint.Expression.Text);
            expressionElement.AppendChild(encodedExpressionNode);
            allDifferentElement.AppendChild(expressionElement);
            constraintsRoot.AppendChild(allDifferentElement);
        }
    }
}