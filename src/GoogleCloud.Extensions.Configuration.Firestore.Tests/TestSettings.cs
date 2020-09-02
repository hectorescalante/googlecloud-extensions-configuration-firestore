using System.Collections.Generic;

namespace Tests.UnitTests
{
  class TestSettings
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Active { get; set; }
    public double Amount { get; set; }
    public TestSettingsDetail Detail  { get; set; }
    public List<TestSettingsDetail> Details { get; set; }
  }

  public class TestSettingsDetail
  {
    public int Status { get; set; }
    public string Description { get; set; }
  }
}
