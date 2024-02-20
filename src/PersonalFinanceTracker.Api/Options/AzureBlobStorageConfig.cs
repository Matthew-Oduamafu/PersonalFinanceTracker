using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618

namespace PersonalFinanceTracker.Api.Options;

public class AzureBlobStorageConfig
{
    [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(string))]
    public string ConnectionString { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(string))]
    public string ImageContainerName { get; set; }
}