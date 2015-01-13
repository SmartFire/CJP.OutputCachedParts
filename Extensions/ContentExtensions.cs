using Orchard.ContentManagement;

namespace CJP.OutputCachedParts.Extensions 
{
    public static class ContentExtensions {
        public static string GetStereotype(this ContentItem contentItem) 
        {
            string stereotype;

            if (!contentItem.TypeDefinition.Settings.TryGetValue("Stereotype", out stereotype)) {
                stereotype = "Content";
            }

            return stereotype;
        }
    }
}