using PawsAndClaws.Models.Entities;

namespace PawsAndClaws.Data
{
    public static class PetSeedData
    {
        public static List<Pet> Pets => new()
        {
            new Pet { Id=1, Name="Megatron", Breed="Aspin", Species="Dog", Age=2, Gender="Male", Size="Medium", Location="Quezon City, PH", Status="Pending", ImageUrl="/images/Megatron.jpg" },
            new Pet { Id=2, Name="Asteroid Destroyer", Breed="Persian", Species="Cat", Age=1, Gender="Female", Size="Small", Location="Quezon City, PH", Status="Available", ImageUrl="/images/Asteroid.jpg" },
            new Pet { Id=3, Name="Josie Megatron", Breed="Persian", Species="Cat", Age=1, Gender="Female", Size="Small", Location="Quezon City, PH", Status="Available", ImageUrl="/images/Josie.png" },
            new Pet { Id=4, Name="Mini Melay", Breed="Chihuahua", Species="Dog", Age=1, Gender="Male", Size="Small", Location="Quezon City, PH", Status="Available", ImageUrl="/images/Melay.jpg" },
            new Pet { Id=5, Name="Marites", Breed="Persian", Species="Cat", Age=3, Gender="Female", Size="Small", Location="Taguig City, PH", Status="Available", ImageUrl="/images/Marites.png" },
            new Pet { Id=6, Name="Monami", Breed="Aspin", Species="Dog", Age=3, Gender="Female", Size="Medium", Location="Quezon City, PH", Status="Pending", ImageUrl="/images/Monami.jpg" },
            new Pet { Id=7, Name="Greg Yapper", Breed="British Shorthair", Species="Cat", Age=1, Gender="Male", Size="Small", Location="Quezon City, PH", Status="Available", ImageUrl="/images/Greg.png" },
            new Pet { Id=8, Name="Galaxy Annihilator", Breed="Persian", Species="Cat", Age=1, Gender="Female", Size="Small", Location="Quezon City, PH", Status="Pending", ImageUrl="/images/Galaxy.jpg" }
        };
    }
}