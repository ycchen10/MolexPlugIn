using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MolexPlugin.Model;


namespace MolexPlugin.DAL
{
    public class AddWorkBuilder
    {
        public AssembleModel Model { get; private set; }

        public AddWorkBuilder()
        {
            AssembleSingleton sl = AssembleSingleton.Instance();
            this.Model = sl.GetAssemble();
        }

        public void CreateBuilder(Matrix4 mat,int workNumber)
        {

        }



    }
}
