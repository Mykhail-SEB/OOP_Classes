namespace OOP_backend_test
{
    public class Vehicle : BaseVehicle
    {
        public Vehicle(int id, string name, string model, string licenceNumber, int mileage, int hourlyFee)
            : base(id, name, model, licenceNumber, mileage, hourlyFee)
        {

        }
        public Vehicle() : base()
            
        {
            
        }
    }
}