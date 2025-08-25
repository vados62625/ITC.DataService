namespace ITC.Domain.Attributes;

[AttributeUsage(validOn: AttributeTargets.Property)]
public class FilterIgnoreAttribute : Attribute
{ }

[AttributeUsage(validOn: AttributeTargets.Property)]
public class FilterCaseInsensitiveAttribute : Attribute
{ }

[AttributeUsage(validOn: AttributeTargets.Property)]
public class FilterSubstringSearchAttribute : Attribute
{ }