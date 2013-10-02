using System;
using System.Web.UI.WebControls;
using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Web.Controls;
using Inedo.BuildMaster.Web.Controls.Extensions;
using Inedo.Web.Controls;

namespace Inedo.BuildMasterExtensions.Rake
{
    internal sealed class RakeActionEditor : ActionEditorBase
    {
        private SourceControlFileFolderPicker txtRakeExecutablePath;
        private SourceControlFileFolderPicker txtWorkingDirectory;
        private ValidatingTextBox txtRakeFile;
        private ValidatingTextBox txtTasks;
        private ValidatingTextBox txtVariableValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="RakeActionEditor"/> class.
        /// </summary>
        public RakeActionEditor()
        {
        }

        public override void BindToForm(ActionBase extension)
        {
            var rakeAction = (RakeAction)extension;

            this.txtRakeExecutablePath.Text = rakeAction.RakeExecutablePath;
            this.txtWorkingDirectory.Text = rakeAction.WorkingDirectory;
            this.txtRakeFile.Text = rakeAction.RakeFile;
            this.txtTasks.Text = rakeAction.Tasks;
            this.txtVariableValues.Text = string.Join(Environment.NewLine, rakeAction.VariableValues ?? new string[0]);
        }

        public override ActionBase CreateFromForm()
        {
            return new RakeAction
            {
                RakeExecutablePath = this.txtRakeExecutablePath.Text,
                WorkingDirectory = this.txtWorkingDirectory.Text,
                RakeFile = this.txtRakeFile.Text,
                Tasks = this.txtTasks.Text,
                VariableValues = this.txtVariableValues.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries),
            };
        }

        protected override void CreateChildControls()
        {
            this.txtRakeExecutablePath = new SourceControlFileFolderPicker
            {
                ServerId = this.ServerId,
                Required = true
            };

            this.txtWorkingDirectory = new SourceControlFileFolderPicker
            {
                ServerId = this.ServerId,
                DefaultText = "default"
            };

            this.txtRakeFile = new ValidatingTextBox { Width = 300 };

            this.txtTasks = new ValidatingTextBox { Width = 300 };

            this.txtVariableValues = new ValidatingTextBox
            {
                TextMode = TextBoxMode.MultiLine,
                Rows = 5,
                Width = 300
            };

            this.Controls.Add(
                new FormFieldGroup("Rake Executable Path",
                    "The path to the Rake executable.",
                    false,
                    new StandardFormField("Rake Executable Path:", this.txtRakeExecutablePath),
                    new StandardFormField("Working Directory:", this.txtWorkingDirectory)
                ),
                new FormFieldGroup("Rake File",
                    "The optional Rake File to use, relative to the working directory.",
                    false,
                    new StandardFormField("Rake File:", this.txtRakeFile)
                ),
                new FormFieldGroup("Tasks",
                    "Enter the tasks to run, separated by spaces.",
                    false,
                    new StandardFormField("Build Properties:", this.txtTasks)
                ),
                new FormFieldGroup("Environment Variables",
                    "You may optionally specify additional environment variables and values for this execution, separated by newlines. For example:<br />opt1=value1<br />opt2=value2",
                    true,
                    new StandardFormField("Environment Variables:", this.txtVariableValues)
                )
            );
        }
    }
}
