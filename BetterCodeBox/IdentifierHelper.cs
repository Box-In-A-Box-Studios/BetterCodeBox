using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BetterCodeBox;

public class IdentifierHelper
{
    public static string GetMethodIdentifier(MethodDeclarationSyntax method)
    {
        string identifier = method.Identifier.ToString();
        // Add Parameters to the identifier
        if (method.ParameterList != null)
        {
            identifier += "(";
            foreach (var parameter in method.ParameterList.Parameters)
            {
                identifier += parameter.Type + " " + parameter.Identifier + ", ";
            }
            identifier = identifier.TrimEnd(',', ' ');
            identifier += ")";
        }
        // Add Class and Namespace to the identifier as a Prefix
        var classDeclaration = method.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        var namespaceDeclaration = method.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
        if (classDeclaration != null)
        {
            identifier = classDeclaration.Identifier + "." + identifier;
        }
        if (namespaceDeclaration != null)
        {
            identifier = namespaceDeclaration.Name + "." + identifier;
        }

        return identifier;
    }
}