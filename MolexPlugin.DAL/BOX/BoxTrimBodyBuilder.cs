using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;

namespace MolexPlugin.DAL
{
    public class BoxTrimBodyBuilder
    {
        private List<Face> faces = new List<Face>();

        private Body body;

        public BoxTrimBodyBuilder(List<Face> faces, Body body)
        {
            this.body = body;
            this.faces = faces;
        }

        public List<NXOpen.Features.TrimBody2> CreateBuilder()
        {
            List<NXOpen.Features.TrimBody2> trims = new List<NXOpen.Features.TrimBody2>();
            foreach (Face face in faces)
            {
                bool isok;
                NXOpen.Features.TrimBody2 trim = TrimBodyUtils.CreateTrimBodyFeature(face, true, out isok, body);
                if(trim!=null)
                {
                    trims.Add(trim);
                    Body temp = trim.GetBodies()[0];
                    if (isok)
                        body = temp;
                }
              
            }
            return trims;
        }
    }
}
