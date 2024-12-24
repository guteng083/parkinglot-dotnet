using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        ParkingLotService parkingLotService = null;

        while (true)
        {
            try
            {
                // Change the prompt to $ with 4 spaces
                Console.Write("$ ");
                var input = Console.ReadLine()?.Split(' ');

                if (input == null || input.Length == 0)
                {
                    Console.WriteLine("Invalid command. Please try again.");
                    continue;
                }

                switch (input[0].ToLower())
                {
                    case "create_parking_lot":
                        if (input.Length != 2 || !int.TryParse(input[1], out int totalSlots) || totalSlots <= 0)
                        {
                            Console.WriteLine("Invalid number of slots. Please provide a positive integer.");
                            break;
                        }

                        parkingLotService = new ParkingLotService(totalSlots);
                        Console.WriteLine($"Created a parking lot with {totalSlots} slots.");
                        break;

                    case "park":
                        if (parkingLotService == null)
                        {
                            Console.WriteLine("Parking lot not created yet.");
                            break;
                        }
                        if (input.Length != 4)
                        {
                            Console.WriteLine("Invalid park command. Format: park <registration_number> <color> <vehicle_type>");
                            break;
                        }

                        var registrationNumber = input[1];
                        var color = input[2];
                        if (!Enum.TryParse<VehicleType>(input[3], true, out var vehicleType))
                        {
                            Console.WriteLine("Invalid vehicle type. It must be either 'Mobil' or 'Motor'.");
                            break;
                        }

                        Console.WriteLine(parkingLotService.ParkVehicle(registrationNumber, color, vehicleType));
                        break;

                    case "leave":
                        if (parkingLotService == null)
                        {
                            Console.WriteLine("Parking lot not created yet.");
                            break;
                        }
                        if (input.Length != 2 || !int.TryParse(input[1], out int slotNumber))
                        {
                            Console.WriteLine("Invalid leave command. Format: leave <slot_number>");
                            break;
                        }

                        Console.WriteLine(parkingLotService.Leave(slotNumber));
                        break;

                    case "status":
                        if (parkingLotService == null)
                        {
                            Console.WriteLine("Parking lot not created yet.");
                            break;
                        }
                        parkingLotService.Status();
                        break;

                    case "type_of_vehicles":
                        if (parkingLotService == null)
                        {
                            Console.WriteLine("Parking lot not created yet.");
                            break;
                        }
                        if (input.Length != 2 || !Enum.TryParse<VehicleType>(input[1], true, out var vehicleTypeForSlot))
                        {
                            Console.WriteLine("Invalid vehicle type. It must be either 'Mobil' or 'Motor'.");
                            break;
                        }
                        parkingLotService.ReportVehicleTypeSlots(vehicleTypeForSlot);
                        break;

                    case "registration_numbers_for_vehicle_with_odd_plate":
                        if (parkingLotService == null)
                        {
                            Console.WriteLine("Parking lot not created yet.");
                            break;
                        }
                        parkingLotService.ReportOddPlateVehicles();
                        break;

                    case "registration_numbers_for_vehicle_with_even_plate":
                        if (parkingLotService == null)
                        {
                            Console.WriteLine("Parking lot not created yet.");
                            break;
                        }
                        parkingLotService.ReportEvenPlateVehicles();
                        break;

                    case "slot_numbers_for_vehicle_with_colour":
                        if (parkingLotService == null)
                        {
                            Console.WriteLine("Parking lot not created yet.");
                            break;
                        }
                        if (input.Length != 2)
                        {
                            Console.WriteLine("Invalid command. Format: slot_numbers_for_vehicle_with_colour <colour>");
                            break;
                        }
                        parkingLotService.ReportSlotNumbersForColour(input[1]);
                        break;

                    case "slot_number_for_registration_number":
                        if (parkingLotService == null)
                        {
                            Console.WriteLine("Parking lot not created yet.");
                            break;
                        }
                        if (input.Length != 2)
                        {
                            Console.WriteLine("Invalid command. Format: slot_number_for_registration_number <registration_number>");
                            break;
                        }
                        parkingLotService.ReportSlotNumberForRegistrationNumber(input[1]);
                        break;

                    case "exit":
                        return;

                    default:
                        Console.WriteLine("Unknown command. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    public enum VehicleType
    {
        Mobil,
        Motor
    }

    public class Vehicle
    {
        public string RegistrationNumber { get; set; }
        public string Color { get; set; }
        public VehicleType Type { get; set; }

        public Vehicle(string registrationNumber, string color, VehicleType type)
        {
            RegistrationNumber = registrationNumber;
            Color = color;
            Type = type;
        }
    }

    public class ParkingSlot
    {
        public int SlotNumber { get; set; }
        public Vehicle Vehicle { get; set; }

        public bool IsOccupied => Vehicle != null;

        public ParkingSlot(int slotNumber)
        {
            SlotNumber = slotNumber;
        }

        public void ParkVehicle(Vehicle vehicle)
        {
            Vehicle = vehicle;
        }

        public void Leave()
        {
            Vehicle = null;
        }
    }

    public class ParkingLotService
    {
        private readonly List<ParkingSlot> _slots;

        public ParkingLotService(int totalSlots)
        {
            _slots = new List<ParkingSlot>();
            for (int i = 1; i <= totalSlots; i++)
            {
                _slots.Add(new ParkingSlot(i));
            }
        }

        public string ParkVehicle(string registrationNumber, string color, VehicleType type)
        {
            var availableSlot = _slots.FirstOrDefault(slot => !slot.IsOccupied);
            if (availableSlot == null) return "Sorry, parking lot is full.";

            var vehicle = new Vehicle(registrationNumber, color, type);
            availableSlot.ParkVehicle(vehicle);
            return $"Allocated slot number: {availableSlot.SlotNumber}";
        }

        public string Leave(int slotNumber)
        {
            var slot = _slots.FirstOrDefault(s => s.SlotNumber == slotNumber);
            if (slot == null)
                return "Invalid slot number.";

            if (!slot.IsOccupied)
                return $"Slot number {slotNumber} is already free.";

            slot.Leave();
            return $"Slot number {slotNumber} is free.";
        }

        public void Status()
        {
            Console.WriteLine("Slot No.\tType\tRegistration No\tColour");
            var occupiedSlots = _slots.Where(s => s.IsOccupied).ToList();

            if (occupiedSlots.Any())
            {
                foreach (var slot in occupiedSlots)
                {
                    Console.WriteLine($"{slot.SlotNumber}\t{slot.Vehicle.Type}\t{slot.Vehicle.RegistrationNumber}\t{slot.Vehicle.Color}");
                }
            }
            else
            {
                Console.WriteLine("No vehicles currently parked.");
            }
        }

        public void ReportVehicleTypeSlots(VehicleType type)
        {
            var slots = _slots.Where(s => s.IsOccupied && s.Vehicle.Type == type).ToList();
            if (!slots.Any())
            {
                Console.WriteLine($"No {type} vehicles found.");
                return;
            }
            Console.WriteLine($"Slots for {type} vehicles:");
            foreach (var slot in slots)
            {
                Console.WriteLine($"Slot No: {slot.SlotNumber}");
            }
        }

        public void ReportOddPlateVehicles()
        {
            var oddPlateVehicles = _slots
                .Where(s => s.IsOccupied && int.TryParse(s.Vehicle.RegistrationNumber, out int plateNumber) && plateNumber % 2 != 0)
                .Select(s => s.Vehicle.RegistrationNumber)
                .ToList();

            if (!oddPlateVehicles.Any())
            {
                Console.WriteLine("No vehicles with odd plate numbers found.");
                return;
            }

            Console.WriteLine("Vehicles with odd plate numbers:");
            foreach (var reg in oddPlateVehicles)
            {
                Console.WriteLine(reg);
            }
        }

        public void ReportEvenPlateVehicles()
        {
            var evenPlateVehicles = _slots
                .Where(s => s.IsOccupied && int.TryParse(s.Vehicle.RegistrationNumber, out int plateNumber) && plateNumber % 2 == 0)
                .Select(s => s.Vehicle.RegistrationNumber)
                .ToList();

            if (!evenPlateVehicles.Any())
            {
                Console.WriteLine("No vehicles with even plate numbers found.");
                return;
            }

            Console.WriteLine("Vehicles with even plate numbers:");
            foreach (var reg in evenPlateVehicles)
            {
                Console.WriteLine(reg);
            }
        }

        public void ReportSlotNumbersForColour(string color)
        {
            var slots = _slots.Where(s => s.IsOccupied && s.Vehicle.Color.Equals(color, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!slots.Any())
            {
                Console.WriteLine($"No vehicles with color {color} found.");
                return;
            }

            Console.WriteLine($"Slots for vehicles with color {color}:");
            foreach (var slot in slots)
            {
                Console.WriteLine($"Slot No: {slot.SlotNumber}");
            }
        }

        public void ReportSlotNumberForRegistrationNumber(string registrationNumber)
        {
            var slot = _slots.FirstOrDefault(s => s.IsOccupied && s.Vehicle.RegistrationNumber.Equals(registrationNumber, StringComparison.OrdinalIgnoreCase));

            if (slot == null)
            {
                Console.WriteLine($"No vehicle found with registration number {registrationNumber}.");
                return;
            }

            Console.WriteLine($"Vehicle with registration number {registrationNumber} is in slot {slot.SlotNumber}.");
        }
    }
}
