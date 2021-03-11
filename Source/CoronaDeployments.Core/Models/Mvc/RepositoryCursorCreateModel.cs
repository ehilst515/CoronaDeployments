using System;
using System.Collections.Generic;
using System.Text;

namespace CoronaDeployments.Core.Models.Mvc
{
    public class RepositoryCursorCreateModel
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string CommitId { get; set; }
        public string CommitMessage { get; set; }
    }
}
