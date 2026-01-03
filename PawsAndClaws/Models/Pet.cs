namespace PawsAndClaws.Models
{
    public class Pet
    {
     
        public required string Name { get; set; }
        public required string Breed { get; set; }
        public int Age { get; set; }
        public required string Gender { get; set; }
        public required string Location { get; set; }
        public required string ImageUrl { get; set; }
        public required string Status { get; set; }
    }
}