using BlazorIdentityMongoDbMatteoFabbri.Data;

namespace BlazorIdentityMongoDbMatteoFabbri.Services
{
    public interface IStudentService
    {
        void SaveOrUpdate(Student student);

        Student GetStudent(string studentId);

        List<Student> GetStudents();

        string Delete(string studentId);
    }
}
