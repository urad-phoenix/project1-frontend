using System.Text.RegularExpressions;

namespace Phoenix.Project1.Editors
{
    public class PathStringUtilities
    {
        public static string AssetPath(string path)
        {
            Regex regex = new Regex(@"Assets.*");
            var match = regex.Match(path);

            return match.Value;
        }          
        
        public static MatchCollection NameStringSplit(string name, string rule)
        {
            //Regex regex = new Regex(@"[^_\d]*");
            var re = string.Format("[^{0}]*", rule);            
            
            Regex regex = new Regex(re);                        
                        
            return regex.Matches(name);
        }
        
        public static string AssetFilterFileType(string fileType)
        {
            Regex regex = new Regex(@"[^.].*");
            var match = regex.Match(fileType);

            return match.Value;
        }        
    }   
}