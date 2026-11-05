using MySqlConnector;
using Recriar.Application.Interfaces;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Context;

namespace Recriar.Infrastructure.Persistence.Repositories;

public class RelationshipTypeRepository : IRelationshipTypeRepository
{
    private readonly AppDbContext _context;

    public RelationshipTypeRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<RelationshipType> GetAll()
    {
        List<RelationshipType> types = new List<RelationshipType>();
        using (MySqlConnection connection = _context.CreateReadConnection())
        {
            connection.Open();
            string sql = "SELECT Id, Name FROM RelationshipTypes ORDER BY Id";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    RelationshipType type = new RelationshipType();
                    type.Id = reader.GetInt32("Id");
                    type.Name = reader.GetString("Name");
                    types.Add(type);
                }
            }
        }
        return types;
    }
}
