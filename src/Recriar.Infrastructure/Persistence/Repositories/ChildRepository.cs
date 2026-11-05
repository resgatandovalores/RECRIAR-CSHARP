using MySqlConnector;
using Recriar.Application.Interfaces;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Context;

namespace Recriar.Infrastructure.Persistence.Repositories;

public class ChildRepository : IChildRepository
{
    private readonly AppDbContext _context;

    public ChildRepository(AppDbContext context)
    {
        _context = context;
    }

    public Child? GetById(int id)
    {
        using (MySqlConnection connection = _context.CreateReadConnection())
        {
            connection.Open();
            // All queries see the same version of this registration.
            using (MySqlTransaction transaction = connection.BeginTransaction(System.Data.IsolationLevel.RepeatableRead))
            {
                Child? child = null;
                string sql = "SELECT * FROM Children WHERE Id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            child = ReadChild(reader);
                        }
                    }
                }
                if (child != null)
                {
                    LoadRelatedData(child, connection, transaction);
                }
                transaction.Commit();
                return child;
            }
        }
    }

    public List<Child> GetAll()
    {
        return ReadList(null);
    }

    public List<Child> SearchByName(string name)
    {
        return ReadList(name);
    }

    private List<Child> ReadList(string? name)
    {
        List<Child> children = new List<Child>();
        using (MySqlConnection connection = _context.CreateReadConnection())
        {
            connection.Open();
            string sql = "SELECT Id, Name, Cpf, Phone FROM Children";
            if (name != null)
            {
                sql = sql + " WHERE LOCATE(@name, Name) > 0";
            }
            sql = sql + " ORDER BY Id";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                if (name != null)
                {
                    command.Parameters.AddWithValue("@name", name);
                }
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Child child = new Child();
                        child.Id = reader.GetInt32("Id");
                        child.Name = reader.GetString("Name");
                        child.Cpf = reader.GetString("Cpf");
                        child.Phone = reader.GetString("Phone");
                        children.Add(child);
                    }
                }
            }
        }
        return children;
    }

    public void Add(Child child)
    {
        CheckRelatedData(child);
        using (MySqlConnection connection = _context.CreateWriteConnection())
        {
            connection.Open();
            using (MySqlTransaction transaction = connection.BeginTransaction())
            {
                string sql = "INSERT INTO Children (Name, Age, Cpf, Rg, Address, Neighborhood, City, School, MotherName, FatherName, Phone, RegistrationDate, HealthCenter, FileNumber, Allergy) VALUES (@Name, @Age, @Cpf, @Rg, @Address, @Neighborhood, @City, @School, @MotherName, @FatherName, @Phone, @RegistrationDate, @HealthCenter, @FileNumber, @Allergy)";
                int id;
                using (MySqlCommand command = new MySqlCommand(sql, connection, transaction))
                {
                    AddChildParameters(command, child);
                    command.ExecuteNonQuery();
                    id = GetNewId(connection, transaction);
                }
                int guardianId = SaveGuardian(child.Guardian!, id, connection, transaction);
                int contactId = SaveEmergencyContact(child.EmergencyContact!, id, connection, transaction);
                List<int> familyIds = SaveFamilyMembers(child, id, connection, transaction, false);
                transaction.Commit();
                ApplyIds(child, id, guardianId, contactId, familyIds);
            }
        }
    }

    public void Update(Child child)
    {
        CheckRelatedData(child);
        using (MySqlConnection connection = _context.CreateWriteConnection())
        {
            connection.Open();
            using (MySqlTransaction transaction = connection.BeginTransaction())
            {
                // Lock the registration while its related records are being changed.
                using (MySqlCommand find = new MySqlCommand("SELECT Id FROM Children WHERE Id = @id FOR UPDATE", connection, transaction))
                {
                    find.Parameters.AddWithValue("@id", child.Id);
                    if (find.ExecuteScalar() == null)
                    {
                        throw new InvalidOperationException("Cadastro não encontrado para edição.");
                    }
                }
                string sql = "UPDATE Children SET Name = @Name, Age = @Age, Cpf = @Cpf, Rg = @Rg, Address = @Address, Neighborhood = @Neighborhood, City = @City, School = @School, MotherName = @MotherName, FatherName = @FatherName, Phone = @Phone, RegistrationDate = @RegistrationDate, HealthCenter = @HealthCenter, FileNumber = @FileNumber, Allergy = @Allergy WHERE Id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection, transaction))
                {
                    AddChildParameters(command, child);
                    command.Parameters.AddWithValue("@id", child.Id);
                    command.ExecuteNonQuery();
                }
                int guardianId = SaveGuardian(child.Guardian!, child.Id, connection, transaction);
                int contactId = SaveEmergencyContact(child.EmergencyContact!, child.Id, connection, transaction);
                List<int> familyIds = SaveFamilyMembers(child, child.Id, connection, transaction, true);
                transaction.Commit();
                ApplyIds(child, child.Id, guardianId, contactId, familyIds);
            }
        }
    }

    public bool Delete(int id)
    {
        using (MySqlConnection connection = _context.CreateWriteConnection())
        {
            connection.Open();
            using (MySqlCommand command = new MySqlCommand("DELETE FROM Children WHERE Id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                int rows = command.ExecuteNonQuery();
                return rows > 0;
            }
        }
    }

    private static Child ReadChild(MySqlDataReader reader)
    {
        Child child = new Child();
        child.Id = reader.GetInt32("Id");
        child.Name = reader.GetString("Name");
        child.Age = reader.GetInt32("Age");
        child.Cpf = reader.GetString("Cpf");
        child.Rg = reader.GetString("Rg");
        child.Address = reader.GetString("Address");
        child.Neighborhood = reader.GetString("Neighborhood");
        child.City = reader.GetString("City");
        child.School = reader.GetString("School");
        child.MotherName = reader.GetString("MotherName");
        child.FatherName = reader.GetString("FatherName");
        child.Phone = reader.GetString("Phone");
        child.RegistrationDate = reader.GetDateTime("RegistrationDate");
        child.HealthCenter = reader.GetString("HealthCenter");
        child.FileNumber = reader.GetString("FileNumber");
        child.Allergy = reader.GetString("Allergy");
        return child;
    }

    private static void AddChildParameters(MySqlCommand command, Child child)
    {
        command.Parameters.AddWithValue("@Name", child.Name);
        command.Parameters.AddWithValue("@Age", child.Age);
        command.Parameters.AddWithValue("@Cpf", child.Cpf);
        command.Parameters.AddWithValue("@Rg", child.Rg);
        command.Parameters.AddWithValue("@Address", child.Address);
        command.Parameters.AddWithValue("@Neighborhood", child.Neighborhood);
        command.Parameters.AddWithValue("@City", child.City);
        command.Parameters.AddWithValue("@School", child.School);
        command.Parameters.AddWithValue("@MotherName", child.MotherName);
        command.Parameters.AddWithValue("@FatherName", child.FatherName);
        command.Parameters.AddWithValue("@Phone", child.Phone);
        command.Parameters.AddWithValue("@RegistrationDate", child.RegistrationDate.Date);
        command.Parameters.AddWithValue("@HealthCenter", child.HealthCenter);
        command.Parameters.AddWithValue("@FileNumber", child.FileNumber);
        command.Parameters.AddWithValue("@Allergy", child.Allergy);
    }

    private static void LoadRelatedData(Child child, MySqlConnection connection, MySqlTransaction transaction)
    {
        using (MySqlCommand command = new MySqlCommand("SELECT * FROM Guardians WHERE ChildId = @id", connection, transaction))
        {
            command.Parameters.AddWithValue("@id", child.Id);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    Guardian guardian = new Guardian();
                    guardian.Id = reader.GetInt32("Id");
                    guardian.ChildId = child.Id;
                    guardian.Child = child;
                    guardian.Name = reader.GetString("Name");
                    guardian.Cpf = reader.GetString("Cpf");
                    guardian.Rg = reader.GetString("Rg");
                    child.Guardian = guardian;
                }
            }
        }
        using (MySqlCommand command = new MySqlCommand("SELECT * FROM EmergencyContacts WHERE ChildId = @id", connection, transaction))
        {
            command.Parameters.AddWithValue("@id", child.Id);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    EmergencyContact contact = new EmergencyContact();
                    contact.Id = reader.GetInt32("Id");
                    contact.ChildId = child.Id;
                    contact.Child = child;
                    contact.Name = reader.GetString("Name");
                    contact.Phone = reader.GetString("Phone");
                    child.EmergencyContact = contact;
                }
            }
        }
        string sql = "SELECT f.*, r.Name AS RelationshipName FROM FamilyMembers f " +
            "JOIN RelationshipTypes r ON r.Id = f.RelationshipTypeId WHERE f.ChildId = @id ORDER BY f.Id";
        using (MySqlCommand command = new MySqlCommand(sql, connection, transaction))
        {
            command.Parameters.AddWithValue("@id", child.Id);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    FamilyMember member = new FamilyMember();
                    member.Id = reader.GetInt32("Id");
                    member.ChildId = child.Id;
                    member.Child = child;
                    member.Name = reader.GetString("Name");
                    member.Age = reader.GetInt32("Age");
                    member.RelationshipTypeId = reader.GetInt32("RelationshipTypeId");
                    if (!reader.IsDBNull(reader.GetOrdinal("OtherRelationship")))
                    {
                        member.OtherRelationship = reader.GetString("OtherRelationship");
                    }
                    RelationshipType type = new RelationshipType();
                    type.Id = member.RelationshipTypeId;
                    type.Name = reader.GetString("RelationshipName");
                    member.RelationshipType = type;
                    child.FamilyMembers.Add(member);
                }
            }
        }
    }

    private static int GetNewId(MySqlConnection connection, MySqlTransaction transaction)
    {
        using (MySqlCommand command = new MySqlCommand("SELECT LAST_INSERT_ID()", connection, transaction))
        {
            return Convert.ToInt32(command.ExecuteScalar());
        }
    }

    private static int FindRelatedId(string sql, int childId, MySqlConnection connection, MySqlTransaction transaction)
    {
        using (MySqlCommand command = new MySqlCommand(sql, connection, transaction))
        {
            command.Parameters.AddWithValue("@childId", childId);
            object? result = command.ExecuteScalar();
            if (result == null)
            {
                return 0;
            }
            return Convert.ToInt32(result);
        }
    }


    private static int SaveGuardian(Guardian person, int childId, MySqlConnection connection, MySqlTransaction transaction)
    {
        int id = FindRelatedId("SELECT Id FROM Guardians WHERE ChildId = @childId", childId, connection, transaction);
        string sql;
        if (id == 0)
        {
            sql = "INSERT INTO Guardians (Name, Cpf, Rg, ChildId) VALUES (@Name, @Cpf, @Rg, @childId)";
        }
        else
        {
            sql = "UPDATE Guardians SET Name = @Name, Cpf = @Cpf, Rg = @Rg WHERE ChildId = @childId";
        }
        using (MySqlCommand command = new MySqlCommand(sql, connection, transaction))
        {
            command.Parameters.AddWithValue("@childId", childId);
            command.Parameters.AddWithValue("@Name", person.Name);
            command.Parameters.AddWithValue("@Cpf", person.Cpf);
            command.Parameters.AddWithValue("@Rg", person.Rg);
            command.ExecuteNonQuery();
            if (id == 0) id = GetNewId(connection, transaction);
        }
        return id;
    }


    private static int SaveEmergencyContact(EmergencyContact person, int childId, MySqlConnection connection, MySqlTransaction transaction)
    {
        int id = FindRelatedId("SELECT Id FROM EmergencyContacts WHERE ChildId = @childId", childId, connection, transaction);
        string sql;
        if (id == 0)
        {
            sql = "INSERT INTO EmergencyContacts (Name, Phone, ChildId) VALUES (@Name, @Phone, @childId)";
        }
        else
        {
            sql = "UPDATE EmergencyContacts SET Name = @Name, Phone = @Phone WHERE ChildId = @childId";
        }
        using (MySqlCommand command = new MySqlCommand(sql, connection, transaction))
        {
            command.Parameters.AddWithValue("@childId", childId);
            command.Parameters.AddWithValue("@Name", person.Name);
            command.Parameters.AddWithValue("@Phone", person.Phone);
            command.ExecuteNonQuery();
            if (id == 0) id = GetNewId(connection, transaction);
        }
        return id;
    }


    private static List<int> SaveFamilyMembers(Child child, int childId, MySqlConnection connection, MySqlTransaction transaction, bool isUpdate)
    {
        List<int> existingIds = new List<int>();
        if (isUpdate)
        {
            using (MySqlCommand command = new MySqlCommand("SELECT Id FROM FamilyMembers WHERE ChildId = @childId", connection, transaction))
            {
                command.Parameters.AddWithValue("@childId", childId);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read()) existingIds.Add(reader.GetInt32("Id"));
                }
            }
        }
        List<int> savedIds = new List<int>();
        foreach (FamilyMember member in child.FamilyMembers)
        {
            int id = member.Id;
            string sql;
            if (isUpdate && id > 0)
            {
                if (!existingIds.Contains(id) || savedIds.Contains(id))
                {
                    throw new InvalidOperationException("Confira os familiares enviados para edição.");
                }
                sql = "UPDATE FamilyMembers SET RelationshipTypeId = @typeId, OtherRelationship = @other, Name = @name, Age = @age WHERE Id = @id AND ChildId = @childId";
            }
            else
            {
                sql = "INSERT INTO FamilyMembers (RelationshipTypeId, OtherRelationship, Name, Age, ChildId) VALUES (@typeId, @other, @name, @age, @childId)";
                id = 0;
            }
            using (MySqlCommand command = new MySqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@typeId", member.RelationshipTypeId);
                command.Parameters.AddWithValue("@other", (object?)member.OtherRelationship ?? DBNull.Value);
                command.Parameters.AddWithValue("@name", member.Name);
                command.Parameters.AddWithValue("@age", member.Age);
                command.Parameters.AddWithValue("@childId", childId);
                if (id > 0) command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
                if (id == 0) id = GetNewId(connection, transaction);
                savedIds.Add(id);
            }
        }
        foreach (int id in existingIds)
        {
            if (!savedIds.Contains(id))
            {
                using (MySqlCommand command = new MySqlCommand("DELETE FROM FamilyMembers WHERE Id = @id AND ChildId = @childId", connection, transaction))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@childId", childId);
                    command.ExecuteNonQuery();
                }
            }
        }
        return savedIds;
    }

    private static void ApplyIds(Child child, int id, int guardianId, int contactId, List<int> familyIds)
    {
        child.Id = id;
        child.Guardian!.Id = guardianId;
        child.Guardian.ChildId = id;
        child.Guardian.Child = child;
        child.EmergencyContact!.Id = contactId;
        child.EmergencyContact.ChildId = id;
        child.EmergencyContact.Child = child;
        for (int i = 0; i < child.FamilyMembers.Count; i++)
        {
            child.FamilyMembers[i].Id = familyIds[i];
            child.FamilyMembers[i].ChildId = id;
            child.FamilyMembers[i].Child = child;
        }
    }

    private static void CheckRelatedData(Child child)
    {
        if (child.Guardian == null || child.EmergencyContact == null)
        {
            throw new InvalidOperationException("Informe o responsável e o contato de emergência.");
        }
    }
}
