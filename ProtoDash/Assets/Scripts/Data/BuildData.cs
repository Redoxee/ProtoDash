using UnityEngine;
using System.Collections;

public class BuildData : ScriptableObject {
	public int Version = 0;
	public int Revision = 0;
	public int Patch = 0;

	const string c_number_format = "000";
	const string c_version_pattern = "{0}_{1}_{2}";
	public string GetVersionLabel()
	{
		return string.Format(c_version_pattern, Version.ToString(c_number_format), Revision.ToString(c_number_format), Patch.ToString(c_number_format));
	}
}
