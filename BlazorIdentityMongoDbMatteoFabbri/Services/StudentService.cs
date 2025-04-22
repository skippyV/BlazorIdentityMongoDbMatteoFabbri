using BlazorIdentityMongoDbMatteoFabbri.Data;
using MongoDB.Driver;

namespace BlazorIdentityMongoDbMatteoFabbri.Services
{
    public class StudentService : IStudentService
    {
        private MongoClient _mongoClient = null;
        private IMongoDatabase _database = null;
        private IMongoCollection<Student> _studentTable = null;
        public StudentService()
        {
            _mongoClient = new MongoClient("mongodb://127.0.0.1:27017/");
            _database = _mongoClient.GetDatabase("SchoolDB");
            _studentTable = _database.GetCollection<Student>("Students");
        }

        public string Delete(string studentId)
        {
            _studentTable.DeleteOne(x => x.Id == studentId);
            return "Deleted";
        }

        public Student GetStudent(string studentId)
        {
            return _studentTable.Find(x => x.Id == studentId).FirstOrDefault();
        }

        public List<Student> GetStudents()
        {
            return _studentTable.Find(FilterDefinition<Student>.Empty).ToList();
        }

        public void SaveOrUpdate(Student student)
        {
            var studentObj = _studentTable.Find(x => x.Id == student.Id).FirstOrDefault();
            if (studentObj == null)
            {
                _studentTable.InsertOne(student);
            }
            else
            {
                _studentTable.ReplaceOne(x => x.Id == student.Id, student);
            }
        }
    }
}
