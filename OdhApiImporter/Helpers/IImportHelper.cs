using DataModel;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiImporter.Helpers
{
    public interface IImportHelper
    {
        Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, CancellationToken cancellationToken = default);        
    }
}
