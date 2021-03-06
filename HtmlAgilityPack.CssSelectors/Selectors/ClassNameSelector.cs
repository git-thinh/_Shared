using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace HapCss.Selectors
{
    internal class ClassNameSelector : CssSelector
    {
        public override string Token
        {
            get { return "."; }
        }

        protected internal override IEnumerable<HtmlAgilityPack.HtmlNode> FilterCore(IEnumerable<HtmlAgilityPack.HtmlNode> currentNodes)
        {
            foreach (var node in currentNodes)
            {
                if (node.GetClassList().Any(c => c.Equals(this.Selector, StringComparison.InvariantCultureIgnoreCase)))
                    yield return node;
            }
        }
    }
}
