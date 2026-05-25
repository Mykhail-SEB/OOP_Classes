using System;

namespace OOP_backend_test
{
    public interface IRentableVehicle
    {
        #region Public properties
        string Name { get; set; }
        string Model { get; set; }
        string LicenceNumber { get; set; }
        int Mileage { get; set; }
        int ID { get; set; }
        float HourlyFee { get; set; }
        float Charge { get; set; }
        CurrentStatus Status { get; set; }
        #endregion

        #region Methods
        string GetName();
        int GetID();
        CurrentStatus GetStatus();
        float GetCharge();
        bool StartRent(int customerID, DateTime startTime);
        bool FinishRent(int customerID, DateTime stopTime);
        bool IsAvailable();
        void StartMaintenance();
        void FinishMaintenance();
        #endregion
    }
}