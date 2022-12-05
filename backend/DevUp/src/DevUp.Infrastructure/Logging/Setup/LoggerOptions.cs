using System.Collections.Generic;
using System.Linq;

namespace DevUp.Infrastructure.Logging.Setup
{
    internal sealed class LoggerOptions
    {
        public ElasticsearchOptions ElasticsearchOptions { get; set; }
        public IEnumerable<string> ExcludePaths { get; set; } = Enumerable.Empty<string>();
    }
}
