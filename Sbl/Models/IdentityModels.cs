using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        // extra fields

        [Display(Name = "Depot")]
        public int? DepotId { get; set; }

        public virtual Depot Depot { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public DbSet<Accident> Accidents { get; set; }
        public DbSet<AccidentFile> AccidentFiles { get; set; }
        public DbSet<Associate> Associates { get; set; }
        public DbSet<AssociateFile> AssociateFiles { get; set; }
        public DbSet<AssociateReceipt> AssociateReceipts { get; set; }
        public DbSet<AssociateRemittance> AssociateRemittances { get; set; }
        public DbSet<Charge> Charges { get; set; }
        public DbSet<ChargeClaim> ChargeClaims { get; set; }
        public DbSet<ChargeClaimFile> ChargeClaimFiles { get; set; }
        public DbSet<ChargeClaimInstalment> ChargeClaimInstalments { get; set; }
        public DbSet<ChargeFile> ChargeFiles { get; set; }
        public DbSet<ChargePcn> ChargePcns { get; set; }
        public DbSet<ChargePcnFile> ChargePcnFiles { get; set; }
        public DbSet<ChargePcnInstalment> ChargePcnInstalments { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Depot> Depots { get; set; }
        public DbSet<Induction> Inductions { get; set; }
        public DbSet<InductionCalendar> InductionCalendars { get; set; }
        public DbSet<InductionNote> InductionNotes { get; set; }
        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<InspectionFile> InspectionFiles { get; set; }
        public DbSet<InspectionCoord> InspectionCoords { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<RentalPriceLog> RentalPriceLogs { get; set; }
        public DbSet<RouteAllocation> RouteAllocations { get; set; }
        public DbSet<RouteAmazon> RouteAmazons { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<SubRental> SubRentals { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleFile> VehicleFiles { get; set; }
        public DbSet<VehicleHandbook> VehicleHandbooks { get; set; }
        public DbSet<VehicleService> VehicleServices { get; set; }
        public DbSet<Finance> Finances { get; set; }
        public DbSet<RouteCharge> RouteCharges { get; set; }
        public DbSet<VehicleRentalPrice> VehicleRentalPrices { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }//end class
}//end namespace