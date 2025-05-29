// Models/AttendanceModels.cs
using System;
using System.Collections.Generic;

namespace APImarcatempoDemo.Models
{
	// Modello per la singola timbratura
	public class AttendanceRecord
	{
		public DateTime DateTime { get; set; } // `dateTime` in camelCase
		public int RecordType { get; set; }    // `recordType` in camelCase
	}

	// Modello per l'utente e la sua timbratura
	public class UserAttendance
	{
		public int Id { get; set; }            // `id` in camelCase
		public string Name { get; set; }       // `name` in camelCase
		public int Password { get; set; }      // `password` in camelCase
		public List<AttendanceRecord> AttendanceRecords { get; set; } // `attendanceRecords` in camelCase
	}

	// Modello della risposta API
	public class ApiResponse
	{
		public List<UserAttendance> Data { get; set; } // `data` in camelCase
	}
}
