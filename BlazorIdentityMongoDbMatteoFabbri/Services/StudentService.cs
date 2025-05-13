using BlazorIdentityMongoDbMatteoFabbri.Data;
using MongoDB.Driver;

namespace BlazorIdentityMongoDbMatteoFabbri.Services
{
    public class StudentService : IStudentService
    {
        private MongoClient mongoClient = null;
        private IMongoDatabase database = null;
        private IMongoCollection<Student> studentTable = null;
        public StudentService()
        {
            mongoClient = new MongoClient("mongodb://127.0.0.1:27017/");
            database = mongoClient.GetDatabase("SchoolDB");
            studentTable = database.GetCollection<Student>("Students");
        }

        public string Delete(string studentId)
        {
            studentTable.DeleteOne(x => x.Id == studentId);
            return "Deleted";
        }

        public Student GetStudent(string studentId)
        {
            return studentTable.Find(x => x.Id == studentId).FirstOrDefault();
        }

        public List<Student> GetStudents()
        {
            return studentTable.Find(FilterDefinition<Student>.Empty).ToList();
        }

        public void SaveOrUpdate(Student student)
        {
            var studentObj = studentTable.Find(x => x.Id == student.Id).FirstOrDefault();
            if (studentObj == null)
            {
                studentTable.InsertOne(student);
            }
            else
            {
                studentTable.ReplaceOne(x => x.Id == student.Id, student);
            }
        }
    }
}
