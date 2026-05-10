namespace RentalPeAPI.User.Domain;

public class User
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }

    public string? Phone { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string Role { get; private set; } = "customer";
    public string? Photo { get; private set; }

    public List<PaymentMethod> PaymentMethods { get; private set; } = new();

    private User() { }

    public User(Guid id, string fullName, string email, string passwordHash,
        string? phone = null, string role = "customer", string? photo = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email no puede estar vacío.", nameof(email));

        Id = id;
        FullName = fullName;
        Email = email;
        PasswordHash = passwordHash;
        Phone = phone;
        CreatedAt = DateTime.UtcNow;
        Role = string.IsNullOrWhiteSpace(role) ? "customer" : role;
        Photo = photo;
    }

    public void AddPaymentMethod(Guid id, string type, string number, string expiry, string cvv)
    {
        var pm = new PaymentMethod(id, Id, type, number, expiry, cvv);
        PaymentMethods.Add(pm);
    }
}

public class PaymentMethod
{
    private PaymentMethod() { }

    public PaymentMethod(Guid id, Guid userId, string type, string number, string expiry, string cvv)
    {
        Id = id;
        UserId = userId;
        Type = type;
        Number = number;
        Expiry = expiry;
        Cvv = cvv;
    }

    public Guid Id { get; private set; }

    // FK al usuario
    public Guid UserId { get; private set; }

    public string Type { get; private set; } = string.Empty;
    public string Number { get; private set; } = string.Empty;
    public string Expiry { get; private set; } = string.Empty;

    // Para que en JSON salga "cvv"
    public string Cvv { get; private set; } = string.Empty;

    // Navegación inversa (opcional pero útil)
    public User? User { get; private set; }
}
