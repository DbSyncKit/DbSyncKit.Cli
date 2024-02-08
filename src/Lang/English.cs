using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbSyncKit.Cli.Lang
{
    public class English
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        #region Text
        public string T_Back => new Lazy<string>(() => "Back").Value;
        public string T_Exit => new Lazy<string>(() => "Exit").Value;

        #endregion

        #region Qustion
        public string Q_Select_Config => new Lazy<string>(() => "Select a Configuration").Value;
        public string Q_Select_Action => new Lazy<string>(() => "Select an Action").Value;
        public string Q_Select_Provider => new Lazy<string>(() => "Select an Database Provider").Value;

        #endregion

        #region Choice

        public string C_EditExistingConfig = new Lazy<string>(() => "Edit Existing Configuration").Value;
        public string C_NewConfig = new Lazy<string>(() => "Add New Configuration").Value;
        public string C_ChangeDefaultDB = new Lazy<string>(() => "Change Default Database").Value;
        public string C_ContractConfig = new Lazy<string>(() => "Configure Data Contract related Configuration").Value;

        #endregion

        #region Message

        public string M_ConfigUpdated = new Lazy<string>(() => "Configuration updated successfully!").Value;
        public string M_ConfigAdded = new Lazy<string>(() => "New configuration added successfully!").Value;

        #endregion

        #region Error
        public string E_PrividerIssue = new Lazy<string>(() => "Provider Does not exists").Value;
        public string E_Initilized = new Lazy<string>(() => "DbSyncKit is not initialized!").Value;
        #endregion


#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
