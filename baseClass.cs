using System;
using System.Reflection;


namespace OOP_backend_test
{
    public class BaseVehicle : IRentableVehicle
    {
        #region Private fields
        private string _name;
        private string _model;
        private string _licenceNumber;
        private int _mileage;
        private int _id;
        private float _hourlyFee;
        private float _charge;
        private CurrentStatus _status;
        #endregion

        #region Public properties
        public string Name
        {
            get => _name;
            set => _name = value;
        }
        public string Model
        {
            get => _model;
            set => _model = value;
        }
        public string LicenceNumber
        {
            get => _licenceNumber;
            set => _licenceNumber = value;
        }
        public int Mileage
        {
            get => _mileage;
            set => _mileage = value;
        }
        public int ID
        {
            get => _id;
            set => _id = value;
        }
        public float HourlyFee
        {
            get => _hourlyFee;
            set => _hourlyFee = value;
        }
        public float Charge
        {
            get => (float)Math.Round(_charge, 2);
            set => _charge = (float)Math.Round(value, 2);
        }
        public CurrentStatus Status
        {
            get => _status;
            set => _status = value;
        }
        #endregion

        #region Constructors
        public BaseVehicle(int id, string name, string model, string licenceNumber, int mileage, float hourlyFee)
        {
            _id = id;
            _name = name;
            _model = model;
            _licenceNumber = licenceNumber;
            _mileage = mileage;
            _hourlyFee = hourlyFee;
            _charge = 0f;
            _status = CurrentStatus.Available;
        }
        public BaseVehicle()
        {
            _id = 2147;
            _name = "DEBUG";
            _model = "THING";
            _licenceNumber = "11111";
            _mileage = 10;
            _hourlyFee = 1;
            _charge = 0f;
            _status = CurrentStatus.Available;
        }
        #endregion

        #region Interface methods
        public string GetName() => _name;
        public int GetID() => _id;
        public CurrentStatus GetStatus() => _status;
        public float GetCharge() => (float)Math.Round(_charge, 2);
        public bool StartRent(int customerID, DateTime startTime)
        {
            if (!IsAvailable())
                return false;

            _status = CurrentStatus.Rented;
            return true;
        }
        public bool FinishRent(int customerID, DateTime stopTime)
        {
            if (_status != CurrentStatus.Rented)
                return false;

            _status = CurrentStatus.Available;
            return true;
        }
        public bool IsAvailable() => _status == CurrentStatus.Available;
        public void StartMaintenance()
        {
            _status = CurrentStatus.InMaintenance;
        }
        public void FinishMaintenance()
        {
            _status = CurrentStatus.Available;
        }
        public override string ToString()
        {
            string RETURNING = "";

            RETURNING += $"Name: {Name}, ";
            RETURNING += $"Model: {Model}, ";
            RETURNING += $"Licence: {LicenceNumber}, ";
            RETURNING += $"Mileage: {Mileage.ToString()}, ";
            RETURNING += $"ID: {ID.ToString()}, ";
            RETURNING += $"Fee: {HourlyFee.ToString()}, ";
            RETURNING += $"Level of charge: {Charge.ToString()}, ";
            RETURNING += $"Status: {Status.ToString()}, ";

            return RETURNING;
        }

        #endregion
    }
}
