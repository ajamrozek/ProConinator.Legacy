using System;
namespace ProConinator.Web.Data
{
    public interface IProConinatorRepository
    {
        void DeleteDocument(string partitionKey, string rowKey);
        System.Collections.Generic.IEnumerable<ProConinator.Web.Models.ProConGroupModel> List(string partitionKey);
        ProConinator.Web.Models.ProConGroupModel Load(string partitionKey, string rowKey);
        ProConinator.Web.Models.ProConGroupModel Save(ProConinator.Web.Models.ProConGroupModel proConGroupModel);
    }
}
