
using BusinessLayer.Services.CargoPricing;
using DataAccessLayer.Context;
using EntityLayer.Constants;
using EntityLayer.Entities;
using EntityLayer.Entities.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MyAcademy_CargoTrack.Seed
{
    public class CargoTrackTestDataSeeder
    {
        private const string TestPassword = "CargoTrack123!";

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            await SeedQuikServiceAsync(context);
            await SeedCitiesAsync(context);
            await SeedBranchesAsync(context);
            await SeedTransferCentersAsync(context);

            await SeedCustomersAsync(context, userManager);
            await SeedManagersAsync(context, userManager);

            await SeedAddressesAsync(context);
            await SeedCargoPricesAsync(context);

            await SeedCargosAsync(context);
            await SeedAboutsAsync(context);
            await SeedContactInfosAsync(context);
        }

        // --------------------------------------------------
        // 1. CITIES
        // --------------------------------------------------

        private static async Task SeedCitiesAsync(AppDbContext context)
        {
            string[] cities =
            {
                "İstanbul", "Ankara", "İzmir", "Manisa",
                "Bursa", "Antalya", "Konya", "Eskişehir"
            };

            foreach (var name in cities)
            {
                if (!await context.Cities.AnyAsync(x => x.Name == name))
                {
                    context.Cities.Add(new City
                    {
                        Name = name
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        // --------------------------------------------------
        // 2. BRANCHES
        // --------------------------------------------------

        private static async Task SeedBranchesAsync(AppDbContext context)
        {
            var data = new[]
            {
                ("İstanbul Merkez Şubesi", "İstanbul"),
                ("İstanbul Kadıköy Şubesi", "İstanbul"),
                ("Ankara Merkez Şubesi", "Ankara"),
                ("Ankara Çankaya Şubesi", "Ankara"),
                ("İzmir Merkez Şubesi", "İzmir"),
                ("İzmir Karşıyaka Şubesi", "İzmir"),
                ("Manisa Merkez Şubesi", "Manisa"),
                ("Manisa Akhisar Şubesi", "Manisa"),
                ("Bursa Merkez Şubesi", "Bursa"),
                ("Antalya Merkez Şubesi", "Antalya"),
                ("Konya Merkez Şubesi", "Konya"),
                ("Eskişehir Merkez Şubesi", "Eskişehir")
            };

            foreach (var item in data)
            {
                var city = await context.Cities
                    .FirstAsync(x => x.Name == item.Item2);

                if (!await context.Branches.AnyAsync(x =>
                    x.Name == item.Item1 && x.CityId == city.Id))
                {
                    context.Branches.Add(new Branch
                    {
                        Name = item.Item1,
                        CityId = city.Id,
                        IsActive = true
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        // --------------------------------------------------
        // 3. TRANSFER CENTERS
        // --------------------------------------------------

        private static async Task SeedTransferCentersAsync(AppDbContext context)
        {
            var data = new[]
            {
                ("İstanbul Transfer Merkezi", "TC-IST-001", "İstanbul"),
                ("Ankara Transfer Merkezi", "TC-ANK-001", "Ankara"),
                ("İzmir Transfer Merkezi", "TC-IZM-001", "İzmir")
            };

            foreach (var item in data)
            {
                if (await context.TransferCenters.AnyAsync(x => x.Code == item.Item2))
                    continue;

                var city = await context.Cities
                    .FirstAsync(x => x.Name == item.Item3);

                context.TransferCenters.Add(new TransferCenter
                {
                    Name = item.Item1,
                    Code = item.Item2,
                    CityId = city.Id,
                    FullAddress = $"{item.Item3} Transfer Merkezi",
                    IsActive = true
                });
            }

            await context.SaveChangesAsync();
        }

        // --------------------------------------------------
        // 4. CUSTOMERS
        // --------------------------------------------------

        private static async Task SeedCustomersAsync(
            AppDbContext context,
            UserManager<AppUser> userManager)
        {
            var existingUsers = await userManager.GetUsersInRoleAsync(RoleNames.User);

            var firstCustomer = existingUsers.FirstOrDefault(x =>
                !(x.Email ?? "").EndsWith("@cargotrack.test"));

            if (firstCustomer == null)
                firstCustomer = existingUsers.FirstOrDefault();

            if (firstCustomer == null)
                throw new Exception("Müşteri oluşturmak için mevcut User hesabı bulunamadı.");

            await EnsureCustomerAsync(
                context, firstCustomer, "CUS-2026-001",
                "İstanbul", "Kadıköy");

            var data = new[]
            {
                ("Ahmet Yılmaz", "customer02@cargotrack.test", "CUS-2026-002", "İstanbul", "Üsküdar"),
                ("Zeynep Demir", "customer03@cargotrack.test", "CUS-2026-003", "Ankara", "Çankaya"),
                ("Mehmet Kaya", "customer04@cargotrack.test", "CUS-2026-004", "İzmir", "Karşıyaka"),
                ("Elif Çelik", "customer05@cargotrack.test", "CUS-2026-005", "Manisa", "Akhisar"),
                ("Burak Şahin", "customer06@cargotrack.test", "CUS-2026-006", "Bursa", "Nilüfer"),
                ("Ayşe Yıldız", "customer07@cargotrack.test", "CUS-2026-007", "Antalya", "Muratpaşa"),
                ("Can Arslan", "customer08@cargotrack.test", "CUS-2026-008", "Konya", "Selçuklu"),
                ("Merve Aydın", "customer09@cargotrack.test", "CUS-2026-009", "Eskişehir", "Tepebaşı"),
                ("Emre Koç", "customer10@cargotrack.test", "CUS-2026-010", "İstanbul", "Beşiktaş"),
                ("Derya Kurt", "customer11@cargotrack.test", "CUS-2026-011", "Ankara", "Yenimahalle"),
                ("Onur Özdemir", "customer12@cargotrack.test", "CUS-2026-012", "İzmir", "Bornova"),
                ("Seda Aslan", "customer13@cargotrack.test", "CUS-2026-013", "Manisa", "Şehzadeler"),
                ("Kerem Polat", "customer14@cargotrack.test", "CUS-2026-014", "Bursa", "Osmangazi"),
                ("Ece Güneş", "customer15@cargotrack.test", "CUS-2026-015", "Antalya", "Konyaaltı")
            };

            foreach (var item in data)
            {
                var user = await userManager.FindByEmailAsync(item.Item2);

                if (user == null)
                {
                    user = new AppUser
                    {
                        FullName = item.Item1,
                        UserName = item.Item2,
                        Email = item.Item2,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, TestPassword);

                    if (!result.Succeeded)
                        throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));
                }

                if (!await userManager.IsInRoleAsync(user, RoleNames.User))
                {
                    var roleResult = await userManager.AddToRoleAsync(user, RoleNames.User);

                    if (!roleResult.Succeeded)
                        throw new Exception("Müşteri rolü atanamadı.");
                }

                await EnsureCustomerAsync(
                    context, user, item.Item3, item.Item4, item.Item5);
            }
        }

        private static async Task EnsureCustomerAsync(
            AppDbContext context,
            AppUser user,
            string customerNumber,
            string city,
            string district)
        {
            if (!await context.Customers.AnyAsync(x => x.AppUserId == user.Id))
            {
                context.Customers.Add(new Customer
                {
                    AppUserId = user.Id,
                    CustomerNumber = customerNumber
                });

                await context.SaveChangesAsync();
            }

            await EnsureAddressAsync(context, user, city, district);
        }

        // --------------------------------------------------
        // 5. MANAGERS / EMPLOYEES
        // --------------------------------------------------

        private static async Task SeedManagersAsync(
            AppDbContext context,
            UserManager<AppUser> userManager)
        {
            var data = new[]
            {
                ("Ali Demir", "manager.istanbul@cargotrack.test", "EMP-2026-001", "İstanbul Merkez Şubesi"),
                ("Fatma Yılmaz", "manager.ankara@cargotrack.test", "EMP-2026-002", "Ankara Merkez Şubesi"),
                ("Murat Kaya", "manager.izmir@cargotrack.test", "EMP-2026-003", "İzmir Merkez Şubesi"),
                ("Ayşe Çelik", "manager.manisa@cargotrack.test", "EMP-2026-004", "Manisa Merkez Şubesi"),
                ("Can Şahin", "manager.bursa@cargotrack.test", "EMP-2026-005", "Bursa Merkez Şubesi")
            };

            foreach (var item in data)
            {
                var user = await userManager.FindByEmailAsync(item.Item2);

                if (user == null)
                {
                    user = new AppUser
                    {
                        FullName = item.Item1,
                        UserName = item.Item2,
                        Email = item.Item2,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, TestPassword);

                    if (!result.Succeeded)
                        throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));
                }

                if (!await userManager.IsInRoleAsync(user, RoleNames.Manager))
                {
                    var roleResult = await userManager.AddToRoleAsync(user, RoleNames.Manager);

                    if (!roleResult.Succeeded)
                        throw new Exception("Manager rolü atanamadı.");
                }

                var branch = await context.Branches
                    .FirstAsync(x => x.Name == item.Item4);

                if (!await context.Employees.AnyAsync(x => x.AppUserId == user.Id))
                {
                    context.Employees.Add(new Employee
                    {
                        AppUserId = user.Id,
                        EmployeeNumber = item.Item3,
                        Position = "Şube Müdürü",
                        BranchId = branch.Id,
                        IsActive = true
                    });

                    await context.SaveChangesAsync();
                }

                await EnsureAddressAsync(
                    context, user, branch.CityId.ToString(), "Merkez");
            }
        }

        // --------------------------------------------------
        // 6. ADDRESSES
        // --------------------------------------------------

        private static async Task SeedAddressesAsync(AppDbContext context)
        {
            var users = await context.Users
                .Where(x => x.Email != null &&
                    x.Email.EndsWith("@cargotrack.test"))
                .ToListAsync();

            foreach (var user in users)
            {
                if (await context.Set<Address>().AnyAsync(x => x.UserId == user.Id))
                    continue;

                context.Set<Address>().Add(new Address
                {
                    UserId = user.Id,
                    Title = "Ev Adresi",
                    City = "İstanbul",
                    District = "Merkez",
                    FullAddress = "Test Mahallesi, Test Sokak No:10"
                });
            }

            await context.SaveChangesAsync();
        }

        private static async Task EnsureAddressAsync(
            AppDbContext context,
            AppUser user,
            string city,
            string district)
        {
            if (await context.Set<Address>().AnyAsync(x => x.UserId == user.Id))
                return;

            context.Set<Address>().Add(new Address
            {
                UserId = user.Id,
                Title = "Ev Adresi",
                City = city,
                District = district,
                FullAddress = $"{district}, {city} - Test Adresi"
            });

            await context.SaveChangesAsync();
        }

        // --------------------------------------------------
        // 7. CARGO PRICES
        // --------------------------------------------------

        private static async Task SeedCargoPricesAsync(AppDbContext context)
        {
            if (await context.CargoPrices.AnyAsync())
                return;

            context.CargoPrices.AddRange(
                new CargoPrice
                {
                    PricePerKg = 25,
                    PricePerDesi = 12,
                    ExtraServicePrice = 40,
                    RegionCoefficient = 1.20m,
                    DeliveryTypeCoefficient = 1.50m,
                    IsActive = true
                },
                new CargoPrice
                {
                    PricePerKg = 20,
                    PricePerDesi = 10,
                    ExtraServicePrice = 30,
                    RegionCoefficient = 1.15m,
                    DeliveryTypeCoefficient = 1.40m,
                    IsActive = false
                },
                new CargoPrice
                {
                    PricePerKg = 18,
                    PricePerDesi = 9,
                    ExtraServicePrice = 25,
                    RegionCoefficient = 1.10m,
                    DeliveryTypeCoefficient = 1.30m,
                    IsActive = false
                });

            await context.SaveChangesAsync();
        }

        // --------------------------------------------------
        // 8. CARGOS + MOVEMENTS + DELIVERIES + EXCEPTIONS
        // --------------------------------------------------

        private static async Task SeedCargosAsync(AppDbContext context)
        {
            if (await context.Cargos.AnyAsync(x => x.TrackCode.StartsWith("MYC-2026-EXTRA-")))
                return;

            var customers = await context.Customers
                .OrderBy(x => x.CustomerNumber)
                .ToListAsync();

            var branches = await context.Branches
                .Include(x => x.City)
                .OrderBy(x => x.Name)
                .ToListAsync();

            var employees = await context.Employees
                .ToListAsync();

            var transferCenters = await context.TransferCenters
                .ToListAsync();

            if (customers.Count < 15 || branches.Count < 5 ||
                employees.Count == 0 || transferCenters.Count == 0)
                throw new Exception("Kargo seed işlemi için gerekli temel kayıtlar bulunamadı.");

            var statuses = new[]
            {
                CargoStatus.Created,
                CargoStatus.AtOriginBranch,
                CargoStatus.InTransferCenter,
                CargoStatus.AtDestinationBranch,
                CargoStatus.OutForDelivery,
                CargoStatus.Delivered,
                CargoStatus.DeliveryFailed,
                CargoStatus.ReturnInProgress,
                CargoStatus.ReturnedToSender
            };

          

            for (int i = 0; i < 60; i++)
            {
                var origin = branches[i % 5];
                var destination = branches[(i + 1) % 5];

                var sender = customers[i % customers.Count];
                var receiver = customers[(i + 1) % customers.Count];

                var finalStatus = statuses[i % statuses.Length];
                var history = GetStatusHistory(finalStatus);

                var shipmentDate = DateTime.UtcNow.AddDays(-(i + 2));

                decimal weight = 1 + (i % 20);
                decimal length = 10 + (i % 15);
                decimal width = 10 + (i % 10);
                decimal height = 10 + (i % 12);
                decimal desi = length * width * height / 3000m;

                decimal pricePerKg = 25;
                decimal pricePerDesi = 12;
                decimal weightPrice = weight * pricePerKg;
                decimal desiPrice = desi * pricePerDesi;

                var cargoType = (CargoType)(i % 3 + 1);
                var deliveryType = i % 2 == 0
                    ? DeliveryType.Standard
                    : DeliveryType.Express;

                decimal extraPrice = cargoType == CargoType.Normal ? 0 : 40;
                decimal regionCoefficient = origin.CityId == destination.CityId ? 1 : 1.20m;
                decimal deliveryCoefficient = deliveryType == DeliveryType.Express ? 1.50m : 1;

                decimal totalPrice =
                    (weightPrice + desiPrice) *
                    regionCoefficient *
                    deliveryCoefficient + extraPrice;

                int failedCount = history.Count(x => x == CargoStatus.DeliveryFailed);

                var cargo = new Cargo
                {
                    TrackCode = $"MYC-2026-EXTRA-{i + 1:000}",
                    ShipmentDate = shipmentDate,
                    EstimatedArrivalDate = shipmentDate.AddDays(
                        deliveryType == DeliveryType.Express ? 2 : 4),
                    ActualDeliveryDate = finalStatus == CargoStatus.Delivered
                        ? shipmentDate.AddDays(2)
                        : null,

                    Weight = weight,
                    Length = length,
                    Width = width,
                    Height = height,
                    Desi = desi,
                    DeclaredValue = 500 + i * 100,

                    CargoType = cargoType,
                    DeliveryType = deliveryType,
                    CargoStatus = finalStatus,
                    FailedDeliveryCount = failedCount,

                    SenderCustomerId = sender.AppUserId,
                    ReceiverCustomerId = receiver.AppUserId,

                    OriginBranchId = origin.Id,
                    DestinationBranchId = destination.Id,

                    PricePerKg = pricePerKg,
                    PricePerDesi = pricePerDesi,
                    WeightPrice = weightPrice,
                    DesiPrice = desiPrice,
                    ExtraServicePrice = extraPrice,
                    TotalPrice = totalPrice,
                    PriceCalculatedAt = shipmentDate
                };

                context.Cargos.Add(cargo);

                await context.SaveChangesAsync();

                await SeedCargoMovementsAsync(
                    context, cargo, history, origin, destination,
                    employees, transferCenters, shipmentDate);

                await SeedDeliveryAsync(context, cargo, finalStatus, i);

                await SeedDeliveryExceptionsAsync(
                     context, cargo, history, employees, shipmentDate, i);

                await SeedCargoAuditAsync(
      context, cargo, sender.AppUserId, history, shipmentDate);
            }

            await context.SaveChangesAsync();
        }

        // --------------------------------------------------
        // 9. STATUS HISTORY
        // --------------------------------------------------

        private static List<CargoStatus> GetStatusHistory(CargoStatus finalStatus)
        {
            var history = new List<CargoStatus>
            {
                CargoStatus.Created,
                CargoStatus.AtOriginBranch,
                CargoStatus.InTransferCenter,
                CargoStatus.AtDestinationBranch,
                CargoStatus.OutForDelivery
            };

            switch (finalStatus)
            {
                case CargoStatus.Created:
                    return history.Take(1).ToList();

                case CargoStatus.AtOriginBranch:
                    return history.Take(2).ToList();

                case CargoStatus.InTransferCenter:
                    return history.Take(3).ToList();

                case CargoStatus.AtDestinationBranch:
                    return history.Take(4).ToList();

                case CargoStatus.OutForDelivery:
                    return history;

                case CargoStatus.Delivered:
                    history.Add(CargoStatus.Delivered);
                    return history;

                case CargoStatus.DeliveryFailed:
                    history.Add(CargoStatus.DeliveryFailed);
                    return history;

                case CargoStatus.ReturnInProgress:
                    history.Add(CargoStatus.DeliveryFailed);
                    history.Add(CargoStatus.DeliveryFailed);
                    history.Add(CargoStatus.ReturnInProgress);
                    return history;

                case CargoStatus.ReturnedToSender:
                    history.Add(CargoStatus.DeliveryFailed);
                    history.Add(CargoStatus.DeliveryFailed);
                    history.Add(CargoStatus.ReturnInProgress);
                    history.Add(CargoStatus.ReturnedToSender);
                    return history;

                default:
                    return history;
            }
        }

        // --------------------------------------------------
        // 10. CARGO MOVEMENTS
        // --------------------------------------------------


        private static async Task SeedCargoMovementsAsync(
            AppDbContext context,
            Cargo cargo,
            List<CargoStatus> history,
            Branch origin,
            Branch destination,
            List<Employee> employees,
            List<TransferCenter> transferCenters,
            DateTime shipmentDate)
        {
            var descriptions = new Dictionary<CargoStatus, string[]>
            {
                [CargoStatus.Created] = new[]
                {
            "Kargo gönderici tarafından sisteme kaydedildi.",
            "Yeni kargo gönderisi oluşturuldu.",
            "Gönderi bilgileri sisteme başarıyla aktarıldı."
        },
                [CargoStatus.AtOriginBranch] = new[]
                {
            "Kargo çıkış şubesine ulaştı.",
            "Kargo çıkış şubesinde teslim alındı.",
            "Gönderi çıkış şubesinde işlem sırasına alındı."
        },
                [CargoStatus.InTransferCenter] = new[]
                {
            "Kargo transfer merkezine ulaştı.",
            "Kargo transfer merkezinde ayrıştırıldı.",
            "Kargo yeni transfer güzergahına yönlendirildi."
        },
                [CargoStatus.AtDestinationBranch] = new[]
                {
            "Kargo varış şubesine ulaştı.",
            "Kargo varış şubesinde teslimat için hazırlandı.",
            "Gönderi dağıtım şubesinde kayıt altına alındı."
        },
                [CargoStatus.OutForDelivery] = new[]
                {
            "Kargo kurye tarafından dağıtıma çıkarıldı.",
            "Kargo teslimat güzergahına alındı.",
            "Gönderi alıcı adresine ulaştırılmak üzere yola çıktı."
        },
                [CargoStatus.Delivered] = new[]
                {
            "Kargo alıcısına başarıyla teslim edildi.",
            "Teslimat işlemi tamamlandı.",
            "Alıcı teslimatı onayladı."
        },
                [CargoStatus.DeliveryFailed] = new[]
                {
            "Teslimat denemesi başarısız oldu.",
            "Kurye teslimat sırasında sorun yaşadı.",
            "Alıcıya ulaşılamadığı için teslimat tamamlanamadı."
        },
                [CargoStatus.ReturnInProgress] = new[]
                {
            "Kargonun göndericiye iade süreci başlatıldı.",
            "İade gönderisi çıkış şubesine yönlendirildi.",
            "Kargo iade operasyonuna alındı."
        },
                [CargoStatus.ReturnedToSender] = new[]
                {
            "Kargo göndericiye iade edildi.",
            "İade işlemi tamamlandı.",
            "Gönderici kargosunu teslim aldı."
        }
            };

            for (int j = 0; j < history.Count; j++)
            {
                var status = history[j];

                Guid? branchId = null;
                Guid? transferCenterId = null;

                if (status == CargoStatus.Created ||
                    status == CargoStatus.AtOriginBranch)
                {
                    branchId = origin.Id;
                }
                else if (status == CargoStatus.InTransferCenter)
                {
                    transferCenterId =
                        transferCenters[j % transferCenters.Count].Id;
                }
                else
                {
                    branchId = destination.Id;

                    if (status == CargoStatus.ReturnInProgress ||
                        status == CargoStatus.ReturnedToSender)
                    {
                        branchId = origin.Id;
                    }
                }

                var employee = branchId.HasValue
                    ? employees.FirstOrDefault(
                        x => x.BranchId == branchId.Value)
                    : null;

                var descriptionList = descriptions[status];

                context.CargoMovements.Add(new CargoMovement
                {
                    CargoId = cargo.Id,

                    OldStatus = j == 0
                        ? null
                        : history[j - 1],

                    NewStatus = status,

                    MovementDate = shipmentDate.AddHours(j * 4),

                    Description = descriptionList[
                        (j + (int)status) % descriptionList.Length],

                    BranchId = branchId,
                    TransferCenterId = transferCenterId,

                    EmployeeId = employee?.AppUserId
                });
            }

            await context.SaveChangesAsync();
        }


        private static string GetMovementDescription(CargoStatus status)
        {
            return status switch
            {
                CargoStatus.Created => "Kargo sisteme oluşturuldu.",
                CargoStatus.AtOriginBranch => "Kargo çıkış şubesine ulaştı.",
                CargoStatus.InTransferCenter => "Kargo transfer merkezine ulaştı.",
                CargoStatus.AtDestinationBranch => "Kargo varış şubesine ulaştı.",
                CargoStatus.OutForDelivery => "Kargo dağıtıma çıkarıldı.",
                CargoStatus.Delivered => "Kargo alıcısına teslim edildi.",
                CargoStatus.DeliveryFailed => "Teslimat denemesi başarısız oldu.",
                CargoStatus.ReturnInProgress => "Kargonun iade süreci başlatıldı.",
                CargoStatus.ReturnedToSender => "Kargo göndericiye iade edildi.",
                _ => "Kargo hareketi kaydedildi."
            };
        }

        // --------------------------------------------------
        // 11. DELIVERIES
        // --------------------------------------------------


        private static async Task SeedDeliveryAsync(
            AppDbContext context,
            Cargo cargo,
            CargoStatus status,
            int index)
        {
            bool deliveryRequired =
                status == CargoStatus.OutForDelivery ||
                status == CargoStatus.Delivered ||
                status == CargoStatus.DeliveryFailed ||
                status == CargoStatus.ReturnInProgress ||
                status == CargoStatus.ReturnedToSender;

            if (!deliveryRequired)
                return;

            var employees = await context.Employees.ToListAsync();

            var recipientNames = new[]
            {
        "Ahmet Yılmaz",
        "Zeynep Demir",
        "Mehmet Kaya",
        "Elif Çelik",
        "Burak Şahin",
        "Ayşe Yıldız",
        "Can Arslan",
        "Merve Aydın"
    };

            var employee = employees.Count > 0
                ? employees[index % employees.Count]
                : null;

            bool delivered = status == CargoStatus.Delivered;

            context.Deliveries.Add(new Delivery
            {
                CargoId = cargo.Id,

                // Eski teslimat kodlarıyla çakışmaması için
                DeliveryCode = (700001 + index).ToString(),

                RecipientName = delivered
                    ? recipientNames[index % recipientNames.Length]
                    : null,

                DeliveredAt = delivered
                    ? cargo.ShipmentDate.AddDays(2 + index % 3)
                    : null,

                EmployeeId = employee?.AppUserId,

                IsCodeVerified = delivered
            });

            await context.SaveChangesAsync();
        }


        // --------------------------------------------------
        // 12. DELIVERY EXCEPTIONS
        // --------------------------------------------------


        private static async Task SeedDeliveryExceptionsAsync(
            AppDbContext context,
            Cargo cargo,
            List<CargoStatus> history,
            List<Employee> employees,
            DateTime shipmentDate,
            int cargoIndex)
        {
            var failedStatuses = history
                .Where(x => x == CargoStatus.DeliveryFailed)
                .ToList();

            if (!failedStatuses.Any())
                return;

            var descriptions = new Dictionary<ExceptionType, string[]>
            {
                [ExceptionType.RecipientNotAvailable] = new[]
                {
            "Alıcı belirtilen adreste bulunamadı.",
            "Kurye adrese ulaştı ancak alıcıya erişilemedi.",
            "Alıcının adreste bulunmaması nedeniyle teslimat yapılamadı."
        },

                [ExceptionType.IncorrectAddress] = new[]
                {
            "Gönderide belirtilen adres hatalı.",
            "Adres bilgileri eksik olduğu için teslimat yapılamadı.",
            "Kurye belirtilen adresi tespit edemedi."
        },

                [ExceptionType.DamagedCargo] = new[]
                {
            "Kargo paketinde taşıma sırasında hasar tespit edildi.",
            "Paketin dış ambalajında deformasyon görüldü.",
            "Gönderi hasarlı olduğu için teslimat durduruldu."
        },

                [ExceptionType.RecipientRejected] = new[]
                {
            "Alıcı kargoyu teslim almayı reddetti.",
            "Alıcı gönderiyi kabul etmedi.",
            "Teslimat sırasında alıcı tarafından gönderi reddedildi."
        },

                [ExceptionType.Other] = new[]
                {
            "Olumsuz hava koşulları nedeniyle teslimat yapılamadı.",
            "Bölgesel operasyonel sorun nedeniyle teslimat ertelendi.",
            "Beklenmeyen operasyonel bir sorun yaşandı."
        }
            };

            for (int i = 0; i < failedStatuses.Count; i++)
            {
                var exceptionType =
                    (ExceptionType)((cargoIndex + i) % 5 + 1);

                var employee = employees.Count > 0
                    ? employees[(cargoIndex + i) % employees.Count]
                    : null;

                var descriptionList = descriptions[exceptionType];

                context.DeliveryExceptions.Add(new DeliveryException
                {
                    CargoId = cargo.Id,

                    DeliveryExceptionType = exceptionType,

                    Description = descriptionList[
                        (cargoIndex + i) % descriptionList.Length],

                    ExceptionDate = shipmentDate.AddDays(i + 1),

                    EmployeeId = employee?.AppUserId
                });
            }

            await context.SaveChangesAsync();
        }


        // --------------------------------------------------
        // 13. AUDIT LOGS
        // --------------------------------------------------


        private static async Task SeedCargoAuditAsync(
            AppDbContext context,
            Cargo cargo,
            Guid userId,
            List<CargoStatus> history,
            DateTime shipmentDate)
        {
            var auditLogs = new List<AuditLog>();

            for (int i = 0; i < history.Count; i++)
            {
                var status = history[i];

                var oldStatus = i == 0
                    ? (CargoStatus?)null
                    : history[i - 1];

                string action = i == 0
                    ? "Create"
                    : status switch
                    {
                        CargoStatus.Delivered => "DeliveryCompleted",
                        CargoStatus.DeliveryFailed => "DeliveryFailed",
                        CargoStatus.ReturnInProgress => "ReturnStarted",
                        CargoStatus.ReturnedToSender => "ReturnCompleted",
                        _ => "StatusChanged"
                    };

                auditLogs.Add(new AuditLog
                {
                    UserId = userId,

                    Action = action,

                    EntityName = "Cargo",

                    EntityId = cargo.Id.ToString(),

                    Description = i == 0
                        ? $"{cargo.TrackCode} takip kodlu kargo oluşturuldu."
                        : $"{cargo.TrackCode} kodlu kargonun durumu " +
                          $"{oldStatus} değerinden {status} değerine değiştirildi.",

                    OldValue = oldStatus.HasValue
                        ? JsonSerializer.Serialize(new
                        {
                            CargoStatus = oldStatus.Value.ToString()
                        })
                        : null,

                    NewValue = JsonSerializer.Serialize(new
                    {
                        CargoStatus = status.ToString(),
                        cargo.TrackCode
                    })
                });
            }

            // Kargo ücret bilgisi için ayrıca kayıt
            auditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = "PriceCalculated",
                EntityName = "Cargo",
                EntityId = cargo.Id.ToString(),

                Description =
                    $"{cargo.TrackCode} kodlu kargonun gönderi ücreti hesaplandı.",

                NewValue = JsonSerializer.Serialize(new
                {
                    cargo.PricePerKg,
                    cargo.PricePerDesi,
                    cargo.WeightPrice,
                    cargo.DesiPrice,
                    cargo.ExtraServicePrice,
                    cargo.TotalPrice
                })
            });

            context.AuditLogs.AddRange(auditLogs);

            await context.SaveChangesAsync();
        }


        // --------------------------------------------------
        // 14. ABOUT
        // --------------------------------------------------

        private static async Task SeedAboutsAsync(AppDbContext context)
        {
            var data = new[]
            {
                ("Hakkımızda", "CargoTrack, güvenilir ve hızlı kargo operasyonları sunmak için geliştirilmiş bir platformdur.", "/images/about.jpg"),
                ("Misyonumuz", "Gönderilerin güvenli ve zamanında teslim edilmesini sağlamak.", "/images/mission.jpg"),
                ("Vizyonumuz", "Teknoloji destekli lojistik çözümlerinde öncü olmak.", "/images/vision.jpg")
            };

            var aboutSet = context.Set<About>();

            if (await aboutSet.AnyAsync())
                return;

            foreach (var item in data)
            {
                aboutSet.Add(new About
                {
                    Title = item.Item1,
                    Description = item.Item2,
                    ImageUrl = item.Item3
                });
            }

            await context.SaveChangesAsync();
        }

        // --------------------------------------------------
        // 15. CONTACT INFO
        // --------------------------------------------------

        private static async Task SeedContactInfosAsync(AppDbContext context)
        {
            var contactSet = context.Set<ContactInfo>();

            if (await contactSet.AnyAsync())
                return;

            contactSet.AddRange(
                new ContactInfo
                {
                    Address = "İstanbul Merkez, Türkiye",
                    EMail = "info@cargotrack.com",
                    PhoneNumber = "02120000001"
                },
                new ContactInfo
                {
                    Address = "Ankara Operasyon Merkezi, Türkiye",
                    EMail = "destek@cargotrack.com",
                    PhoneNumber = "03120000002"
                });

            await context.SaveChangesAsync();
        }

        private static async Task SeedQuikServiceAsync(AppDbContext context)
        {
           

            var quikServiceSet = context.Set<QuikService>();
            if (await quikServiceSet.AnyAsync())
                return;

            quikServiceSet.AddRange(
              new QuikService
              {
                  IconUrl = "track_changes",
                  Title = "Kargo Takibi",
                  Description = "Kargonuzun güncel konumunu ve teslimat sürecini takip edin."
              },
              new QuikService
              {
                  IconUrl = "add_box",
                  Title = "Yeni Kargo Gönder",
                  Description = "Göndermek istediğiniz kargo için kolayca gönderi oluşturun."
              },
            new QuikService
            {
                IconUrl = "inventory_2",
                Title = "Gönderilerim",
                Description = "Oluşturduğunuz tüm kargo gönderilerini tek ekranda görüntüleyin."
            },
              new QuikService
              {
                  IconUrl = "outbox",
                  Title = "Giden Kargolarım",
                  Description = "Gönderdiğiniz kargoların hareketlerini ve teslimat süreçlerini takip edin."
              });

            await context.SaveChangesAsync();
        }
    }
}
