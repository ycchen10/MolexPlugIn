using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;


namespace Basic
{
    class SelectionRuleFactory
    {
        private List<Point> points = new List<Point>();
        private List<Face> faces = new List<Face>();
        private List<Edge> edges = new List<Edge>();
        private List<Body> bodys = new List<Body>();
        private List<Curve> curves = new List<Curve>();
        public SelectionRuleFactory(List<TaggedObject> taggedObjects)
        {
            this.points = taggedObjects.Where(a => a is Point).Select(a => a as Point).ToList();
            this.faces = taggedObjects.Where(a => a is Face).Select(a => a as Face).ToList();
            this.edges = taggedObjects.Where(a => a is Edge).Select(a => a as Edge).ToList();
            this.bodys = taggedObjects.Where(a => a is Body).Select(a => a as Body).ToList();
            this.curves = taggedObjects.Where(a => a is Curve).Select(a => a as Curve).ToList();

        }

        public List<SelectionIntentRule> CreateSelectionRule()
        {
            List<SelectionIntentRule> rules = new List<SelectionIntentRule>();
            if (points != null && points.Count != 0)
            {
                rules.Add(new SelectionCurveFromPointRule(points).CreateSelectionRule());
            }
            if (faces != null && faces.Count != 0)
            {
                rules.Add(new SelectionFaceRule(faces).CreateSelectionRule());
            }
            if (edges != null && edges.Count != 0)
            {
                rules.Add(new SelectionEdgeRule(edges).CreateSelectionRule());
            }
            if (bodys != null && bodys.Count != 0)
            {
                rules.Add(new SelectionBodyRule(bodys).CreateSelectionRule());
            }
            if (curves != null && curves.Count != 0)
            {
                rules.Add(new SelectionCurveRule(curves).CreateSelectionRule());
            }
            return rules;
        }

    }
}
