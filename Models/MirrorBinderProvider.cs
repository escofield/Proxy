using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace RequestIntercessor.Models
{
    public class MirrorBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(MirrorModel))
                return new MirrorBinder();
            return null;
        }
    }
}
