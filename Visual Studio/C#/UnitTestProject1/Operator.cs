using System;
using Eruru.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1 {

	[TestClass]
	public class Operator {

		[TestMethod]
		public void Equals () {
			Assert.IsTrue (new JsonValue (66) == (byte)66);
			Assert.IsTrue (new JsonValue (66) == (ushort)66);
			Assert.IsTrue (new JsonValue (66) == (uint)66);
			Assert.IsTrue (new JsonValue (66) == (ulong)66);
			Assert.IsTrue (new JsonValue (66) == (sbyte)66);
			Assert.IsTrue (new JsonValue (66) == (short)66);
			Assert.IsTrue (new JsonValue (66) == 66);
			Assert.IsTrue (new JsonValue (66) == (long)66);
			Assert.IsTrue (new JsonValue (true) == true);
			Assert.IsTrue (new JsonValue (66.5) == (float)66.5);
			Assert.IsTrue (new JsonValue (66.5) == (double)66.5);
			Assert.IsTrue (new JsonValue (66.5) == (decimal)66.5);
			Assert.IsTrue (new JsonValue (66) == (char)66);
			Assert.IsTrue (new JsonValue (66) == "66");
			Assert.IsTrue (new JsonValue (DateTime.MinValue) == DateTime.MinValue);
			Assert.IsTrue (new JsonValue (new JsonArray (66)) == new JsonArray (66));
			Assert.IsTrue (new JsonValue (new JsonObject () { { "Number", 66 } }) == new JsonObject () { { "Number", 66 } });
			Assert.IsTrue ((byte)66 == new JsonValue (66));
			Assert.IsTrue ((ushort)66 == new JsonValue (66));
			Assert.IsTrue ((uint)66 == new JsonValue (66));
			Assert.IsTrue ((ulong)66 == new JsonValue (66));
			Assert.IsTrue ((sbyte)66 == new JsonValue (66));
			Assert.IsTrue ((short)66 == new JsonValue (66));
			Assert.IsTrue (66 == new JsonValue (66));
			Assert.IsTrue ((long)66 == new JsonValue (66));
			Assert.IsTrue (true == new JsonValue (true));
			Assert.IsTrue ((float)66.5 == new JsonValue (66.5));
			Assert.IsTrue ((double)66.5 == new JsonValue (66.5));
			Assert.IsTrue ((decimal)66.5 == new JsonValue (66.5));
			Assert.IsTrue ((char)66 == new JsonValue (66));
			Assert.IsTrue ("66" == new JsonValue (66));
			Assert.IsTrue (DateTime.MinValue == new JsonValue (DateTime.MinValue));
			Assert.IsTrue (new JsonArray (66) == new JsonValue (new JsonArray (66)));
			Assert.IsTrue (new JsonObject () { { "Number", 66 } } == new JsonValue (new JsonObject () { { "Number", 66 } }));
		}

		[TestMethod]
		public void NotEquals () {
			Assert.IsTrue (new JsonValue (66) != (byte)67);
			Assert.IsTrue (new JsonValue (66) != (ushort)67);
			Assert.IsTrue (new JsonValue (66) != (uint)67);
			Assert.IsTrue (new JsonValue (66) != (ulong)67);
			Assert.IsTrue (new JsonValue (66) != (sbyte)67);
			Assert.IsTrue (new JsonValue (66) != (short)67);
			Assert.IsTrue (new JsonValue (66) != 67);
			Assert.IsTrue (new JsonValue (66) != (long)67);
			Assert.IsTrue (new JsonValue (true) != false);
			Assert.IsTrue (new JsonValue (66.5) != (float)67.5);
			Assert.IsTrue (new JsonValue (66.5) != (double)67.5);
			Assert.IsTrue (new JsonValue (66.5) != (decimal)67.5);
			Assert.IsTrue (new JsonValue (66) != (char)67);
			Assert.IsTrue (new JsonValue (66) != "67");
			Assert.IsTrue (new JsonValue (DateTime.MaxValue) != DateTime.MinValue);
			Assert.IsTrue (new JsonValue (new JsonArray (66)) != new JsonArray (67));
			Assert.IsTrue (new JsonValue (new JsonObject () { { "Number", 66 } }) != new JsonObject () { { "Number", 67 } });
			Assert.IsTrue ((byte)66 != new JsonValue (67));
			Assert.IsTrue ((ushort)66 != new JsonValue (67));
			Assert.IsTrue ((uint)66 != new JsonValue (67));
			Assert.IsTrue ((ulong)66 != new JsonValue (67));
			Assert.IsTrue ((sbyte)66 != new JsonValue (67));
			Assert.IsTrue ((short)66 != new JsonValue (67));
			Assert.IsTrue (66 != new JsonValue (67));
			Assert.IsTrue ((long)66 != new JsonValue (67));
			Assert.IsTrue (false != new JsonValue (true));
			Assert.IsTrue ((float)66.5 != new JsonValue (67.5));
			Assert.IsTrue ((double)66.5 != new JsonValue (67.5));
			Assert.IsTrue ((decimal)66.5 != new JsonValue (67.5));
			Assert.IsTrue ((char)66 != new JsonValue (67));
			Assert.IsTrue ("66" != new JsonValue (67));
			Assert.IsTrue (DateTime.MaxValue != new JsonValue (DateTime.MinValue));
			Assert.IsTrue (new JsonArray (66) != new JsonValue (new JsonArray (67)));
			Assert.IsTrue (new JsonObject () { { "Number", 66 } } != new JsonValue (new JsonObject () { { "Number", 67 } }));
		}

		[TestMethod]
		public void GreaterThan () {
			Assert.IsTrue (new JsonValue (66) > (byte)65);
			Assert.IsTrue (new JsonValue (66) > (ushort)65);
			Assert.IsTrue (new JsonValue (66) > (uint)65);
			Assert.IsTrue (new JsonValue (66) > (ulong)65);
			Assert.IsTrue (new JsonValue (66) > (sbyte)65);
			Assert.IsTrue (new JsonValue (66) > (short)65);
			Assert.IsTrue (new JsonValue (66) > 65);
			Assert.IsTrue (new JsonValue (66) > (long)65);
			Assert.IsTrue (new JsonValue (66.5) > (float)65.5);
			Assert.IsTrue (new JsonValue (66.5) > (double)65.5);
			Assert.IsTrue (new JsonValue (66.5) > (decimal)65.5);
			Assert.IsTrue (new JsonValue (66) > (char)65);
			Assert.IsTrue (new JsonValue (66) > "65");
			Assert.IsTrue (new JsonValue (DateTime.MaxValue) > DateTime.MinValue);
			Assert.IsTrue ((byte)66 > new JsonValue (65));
			Assert.IsTrue ((ushort)66 > new JsonValue (65));
			Assert.IsTrue ((uint)66 > new JsonValue (65));
			Assert.IsTrue ((ulong)66 > new JsonValue (65));
			Assert.IsTrue ((sbyte)66 > new JsonValue (65));
			Assert.IsTrue ((short)66 > new JsonValue (65));
			Assert.IsTrue (66 > new JsonValue (65));
			Assert.IsTrue ((long)66 > new JsonValue (65));
			Assert.IsTrue ((float)66.5 > new JsonValue (65.5));
			Assert.IsTrue ((double)66.5 > new JsonValue (65.5));
			Assert.IsTrue ((decimal)66.5 > new JsonValue (65.5));
			Assert.IsTrue ((char)66 > new JsonValue (65));
			Assert.IsTrue ("66" > new JsonValue (65));
			Assert.IsTrue (DateTime.MaxValue > new JsonValue (DateTime.MinValue));
		}

		[TestMethod]
		public void LessThan () {
			Assert.IsTrue (new JsonValue (66) < (byte)67);
			Assert.IsTrue (new JsonValue (66) < (ushort)67);
			Assert.IsTrue (new JsonValue (66) < (uint)67);
			Assert.IsTrue (new JsonValue (66) < (ulong)67);
			Assert.IsTrue (new JsonValue (66) < (sbyte)67);
			Assert.IsTrue (new JsonValue (66) < (short)67);
			Assert.IsTrue (new JsonValue (66) < 67);
			Assert.IsTrue (new JsonValue (66) < (long)67);
			Assert.IsTrue (new JsonValue (66.5) < (float)67.5);
			Assert.IsTrue (new JsonValue (66.5) < (double)67.5);
			Assert.IsTrue (new JsonValue (66.5) < (decimal)67.5);
			Assert.IsTrue (new JsonValue (66) < (char)67);
			Assert.IsTrue (new JsonValue (66) < "67");
			Assert.IsTrue (new JsonValue (DateTime.MinValue) < DateTime.MaxValue);
			Assert.IsTrue ((byte)66 < new JsonValue (67));
			Assert.IsTrue ((ushort)66 < new JsonValue (67));
			Assert.IsTrue ((uint)66 < new JsonValue (67));
			Assert.IsTrue ((ulong)66 < new JsonValue (67));
			Assert.IsTrue ((sbyte)66 < new JsonValue (67));
			Assert.IsTrue ((short)66 < new JsonValue (67));
			Assert.IsTrue (66 < new JsonValue (67));
			Assert.IsTrue ((long)66 < new JsonValue (67));
			Assert.IsTrue ((float)66.5 < new JsonValue (67.5));
			Assert.IsTrue ((double)66.5 < new JsonValue (67.5));
			Assert.IsTrue ((decimal)66.5 < new JsonValue (67.5));
			Assert.IsTrue ((char)66 < new JsonValue (67));
			Assert.IsTrue ("66" < new JsonValue (67));
			Assert.IsTrue (DateTime.MinValue < new JsonValue (DateTime.MaxValue));
		}

		[TestMethod]
		public void Add () {
			Assert.AreEqual (100, new JsonValue (66) + (byte)34);
			Assert.AreEqual (100, new JsonValue (66) + (ushort)34);
			Assert.AreEqual ((uint)100, new JsonValue (66) + (uint)34);
			Assert.AreEqual (100UL, new JsonValue (66) + (ulong)34);
			Assert.AreEqual (100, new JsonValue (66) + (sbyte)34);
			Assert.AreEqual (100, new JsonValue (66) + (short)34);
			Assert.AreEqual (100, new JsonValue (66) + 34);
			Assert.AreEqual (100L, new JsonValue (66) + (long)34);
			Assert.AreEqual (100.9F, new JsonValue (66.5) + (float)34.4);
			Assert.AreEqual (100.9D, new JsonValue (66.5) + (double)34.4);
			Assert.AreEqual (100.9M, new JsonValue (66.5) + (decimal)34.4);
			Assert.AreEqual ((char)100, new JsonValue (66) + (char)34);
			Assert.AreEqual ("6634", new JsonValue (66) + "34");
			Assert.AreEqual (100, (byte)66 + new JsonValue (34));
			Assert.AreEqual (100, (ushort)66 + new JsonValue (34));
			Assert.AreEqual ((uint)100, (uint)66 + new JsonValue (34));
			Assert.AreEqual (100UL, (ulong)66 + new JsonValue (34));
			Assert.AreEqual (100, (sbyte)66 + new JsonValue (34));
			Assert.AreEqual (100, (short)66 + new JsonValue (34));
			Assert.AreEqual (100, 66 + new JsonValue (34));
			Assert.AreEqual (100L, (long)66 + new JsonValue (34));
			Assert.AreEqual (100F, (float)66 + new JsonValue (34));
			Assert.AreEqual (100D, (double)66 + new JsonValue (34));
			Assert.AreEqual (100M, (decimal)66 + new JsonValue (34));
			Assert.AreEqual ((char)100, (char)66 + new JsonValue (34));
			Assert.AreEqual ("6634", "66" + new JsonValue (34));
		}

		[TestMethod]
		public void Subtract () {
			Assert.AreEqual (32, new JsonValue (66) - (byte)34);
			Assert.AreEqual (32, new JsonValue (66) - (ushort)34);
			Assert.AreEqual ((uint)32, new JsonValue (66) - (uint)34);
			Assert.AreEqual (32UL, new JsonValue (66) - (ulong)34);
			Assert.AreEqual (32, new JsonValue (66) - (sbyte)34);
			Assert.AreEqual (32, new JsonValue (66) - (short)34);
			Assert.AreEqual (32, new JsonValue (66) - 34);
			Assert.AreEqual (32L, new JsonValue (66) - (long)34);
			Assert.AreEqual ("32.2", (new JsonValue (66.6) - (float)34.4).ToString ());
			Assert.AreEqual ("32.2", (new JsonValue (66.6) - (double)34.4).ToString ());
			Assert.AreEqual ("32.2", (new JsonValue (66.6) - (decimal)34.4).ToString ());
			Assert.AreEqual ((char)32, new JsonValue (66) - (char)34);
			Assert.AreEqual (32, (byte)66 - new JsonValue (34));
			Assert.AreEqual (32, (ushort)66 - new JsonValue (34));
			Assert.AreEqual ((uint)32, (uint)66 - new JsonValue (34));
			Assert.AreEqual (32UL, (ulong)66 - new JsonValue (34));
			Assert.AreEqual (32, (sbyte)66 - new JsonValue (34));
			Assert.AreEqual (32, (short)66 - new JsonValue (34));
			Assert.AreEqual (32, 66 - new JsonValue (34));
			Assert.AreEqual (32L, (long)66 - new JsonValue (34));
			Assert.AreEqual (32F, (float)66 - new JsonValue (34));
			Assert.AreEqual (32D, (double)66 - new JsonValue (34));
			Assert.AreEqual (32M, (decimal)66 - new JsonValue (34));
			Assert.AreEqual ((char)32, (char)66 - new JsonValue (34));
		}

		[TestMethod]
		public void Multiply () {
			Assert.AreEqual (2244, new JsonValue (66) * (byte)34);
			Assert.AreEqual (2244, new JsonValue (66) * (ushort)34);
			Assert.AreEqual ((uint)2244, new JsonValue (66) * (uint)34);
			Assert.AreEqual (2244UL, new JsonValue (66) * (ulong)34);
			Assert.AreEqual (2244, new JsonValue (66) * (sbyte)34);
			Assert.AreEqual (2244, new JsonValue (66) * (short)34);
			Assert.AreEqual (2244, new JsonValue (66) * 34);
			Assert.AreEqual (2244L, new JsonValue (66) * (long)34);
			Assert.AreEqual ("2291.04", (new JsonValue (66.6) * (float)34.4).ToString ());
			Assert.AreEqual ("2291.04", (new JsonValue (66.6) * (double)34.4).ToString ());
			Assert.AreEqual ("2291.04", (new JsonValue (66.6) * (decimal)34.4).ToString ());
			Assert.AreEqual ((char)2244, new JsonValue (66) * (char)34);
			Assert.AreEqual (2244, (byte)66 * new JsonValue (34));
			Assert.AreEqual (2244, (ushort)66 * new JsonValue (34));
			Assert.AreEqual ((uint)2244, (uint)66 * new JsonValue (34));
			Assert.AreEqual (2244UL, (ulong)66 * new JsonValue (34));
			Assert.AreEqual (2244, (sbyte)66 * new JsonValue (34));
			Assert.AreEqual (2244, (short)66 * new JsonValue (34));
			Assert.AreEqual (2244, 66 * new JsonValue (34));
			Assert.AreEqual (2244L, (long)66 * new JsonValue (34));
			Assert.AreEqual (2244F, (float)66 * new JsonValue (34));
			Assert.AreEqual (2244D, (double)66 * new JsonValue (34));
			Assert.AreEqual (2244M, (decimal)66 * new JsonValue (34));
			Assert.AreEqual ((char)2244, (char)66 * new JsonValue (34));
		}

		[TestMethod]
		public void Divide () {
			Assert.AreEqual (2, new JsonValue (60) / (byte)30);
			Assert.AreEqual (2, new JsonValue (60) / (ushort)30);
			Assert.AreEqual ((uint)2, new JsonValue (60) / (uint)30);
			Assert.AreEqual (2UL, new JsonValue (60) / (ulong)30);
			Assert.AreEqual (2, new JsonValue (60) / (sbyte)30);
			Assert.AreEqual (2, new JsonValue (60) / (short)30);
			Assert.AreEqual (2, new JsonValue (60) / 30);
			Assert.AreEqual (2L, new JsonValue (60) / (long)30);
			Assert.AreEqual ("1.1", (new JsonValue (66.11) / (float)60.1).ToString ());
			Assert.AreEqual ("1.1", (new JsonValue (66.11) / (double)60.1).ToString ());
			Assert.AreEqual ("1.1", (new JsonValue (66.11) / (decimal)60.1).ToString ());
			Assert.AreEqual ((char)2, new JsonValue (60) / (char)30);
			Assert.AreEqual (2, (byte)60 / new JsonValue (30));
			Assert.AreEqual (2, (ushort)60 / new JsonValue (30));
			Assert.AreEqual ((uint)2, (uint)60 / new JsonValue (30));
			Assert.AreEqual (2UL, (ulong)60 / new JsonValue (30));
			Assert.AreEqual (2, (sbyte)60 / new JsonValue (30));
			Assert.AreEqual (2, (short)60 / new JsonValue (30));
			Assert.AreEqual (2, 60 / new JsonValue (30));
			Assert.AreEqual (2L, (long)60 / new JsonValue (30));
			Assert.AreEqual (2F, (float)60 / new JsonValue (30));
			Assert.AreEqual (2D, (double)60 / new JsonValue (30));
			Assert.AreEqual (2M, (decimal)60 / new JsonValue (30));
			Assert.AreEqual ((char)2, (char)60 / new JsonValue (30));
		}

	}

}