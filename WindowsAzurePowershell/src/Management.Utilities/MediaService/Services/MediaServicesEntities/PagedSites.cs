using System.Runtime.Serialization;
using Microsoft.WindowsAzure.Management.Utilities.Websites.Services.WebEntities;

namespace Microsoft.WindowsAzure.Management.Utilities.MediaService.Services.MediaServicesEntities
{
    /// <summary>
    ///     Paged collection of sites
    /// </summary>
    [DataContract(Namespace = MediaServicesUriElements.ServiceNamespace)]
    public class PagedSites : PagedSet<MediaServiceAccount>
    {
    }
}