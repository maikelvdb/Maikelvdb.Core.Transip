namespace Maikelvdb.Core.Transip.Api.Models.Options
{
    public class TransipApiOptions
    {
        public bool HasPrivateKeyPath { get; private set; }
        private string _privateKeyPath { get; set; }

        public string PrivateKeyPath
        {
            get => _privateKeyPath;
            set
            {
                HasPrivateKeyPath = string.IsNullOrEmpty(value) ? false : true;
                _privateKeyPath = value;
            }
        }

        public bool IsTest { get; set; }
    }
}
