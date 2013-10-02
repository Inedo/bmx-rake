using System;
using System.Collections.Generic;
using System.IO;
using Inedo.BuildMaster;
using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Extensibility.Agents;
using Inedo.BuildMaster.Web;

namespace Inedo.BuildMasterExtensions.Rake
{
    [ActionProperties(
       "Execute Rake",
       "Runs the Rake executable.",
       "Ruby")]
    [CustomEditor(typeof(RakeActionEditor))]
    public sealed class RakeAction : AgentBasedActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RakeAction"/> class.
        /// </summary>
        public RakeAction()
        {
        }

        /// <summary>
        /// Gets or sets the path to the Rake executable.
        /// </summary>
        [Persistent]
        public string RakeExecutablePath { get; set; }

        /// <summary>
        /// The optional Rake file to use.
        /// </summary>
        [Persistent]
        public string RakeFile { get; set; }

        /// <summary>
        /// The working directory of the executable.
        /// </summary>
        [Persistent]
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Gets or sets the environment variables and their values in the form: VAR=VALUE.
        /// </summary>
        [Persistent]
        public string[] VariableValues { get; set; }

        /// <summary>
        /// Gets or sets the Rake tasks to run.
        /// </summary>
        [Persistent]
        public string Tasks { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "Execute the Rake task{0} \"{1}\"{2}",
                (this.Tasks.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length == 1) ? String.Empty : "s",
                string.IsNullOrEmpty(this.Tasks) ? "default" : this.Tasks,
                Util.ConcatNE(" using the Rake File: \"", this.RakeFile, "\"")
            );
        }

        /// <summary>
        /// This method is called to execute the Action.
        /// </summary>
        protected override void Execute()
        {
            this.LogInformation("Executing Rake...");
            this.ExecuteCommandLine(this.RakeExecutablePath, this.BuildArguments(), this.GetAbsoluteWorkingDirectory());
            this.LogInformation("Rake execution complete.");
        }

        /// <summary>
        /// Builds the arguments to be passed into the Rake executable based on the properties of this action.
        /// </summary>
        private string BuildArguments()
        {
            var args = new List<string>();

            args.Add("--quiet");

            if (!string.IsNullOrEmpty(this.RakeFile))
                args.Add(string.Format("--rakefile \"{0}\"", this.RakeFile));

            foreach (string variableValue in this.VariableValues ?? new string[0])
                args.Add(string.Format("\"{0}\"", variableValue));

            args.Add(this.Tasks ?? string.Empty);

            return string.Join(" ", args.ToArray());
        }

        private string GetAbsoluteWorkingDirectory()
        {
            var agent = this.Context.Agent.GetService<IFileOperationsExecuter>();

            if (string.IsNullOrEmpty(this.WorkingDirectory))
                return this.Context.SourceDirectory;
            else if (Path.IsPathRooted(this.WorkingDirectory))
                return this.WorkingDirectory;
            else if (this.WorkingDirectory.StartsWith("~"))
                return agent.CombinePath(
                    agent.GetDefaultApplicationBaseDirectory(this.Context.ApplicationId),
                    this.WorkingDirectory.Substring(1).TrimStart(agent.GetDirectorySeparator())
                );
            else
                return Util.Path2.Combine(this.Context.SourceDirectory, this.WorkingDirectory);
        }
    }
}
