using EmployeeManagement.DataAccess.Entities;

namespace EmployeeManagement.Test;

public class CourseTests
{
    [Fact]
    public void CourseConstructor_ConstructCourse_IsNewMustBeTrue()
    {
        // Nothing to arrange
        
        // Act 
        var course = new Course("Disaster Management 101");
        
        // Assert
        Assert.True(course.IsNew);
    }
}