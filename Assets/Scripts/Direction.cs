///<summary>
///Holds directional information for pieces and their movements
///Combining directional calls allows things like Direction.Left.Up which means Up and to the left
///</summary>
public class Direction {
	//0000 - none
	//0001 - left
	//0010 - right
	//0100 - up
	//1000 - down 
	private byte direction = 0;

	private Direction(byte direction) {
		this.direction = direction;
	}

	private Direction(Direction other) {
		direction = other.direction;
	}

	///<summary>Returns a new Direction pointing nowhere</summary>
	public static Direction None {
		get {
			return new Direction(0);
		}
	}

	///<summary>Returns a new Direction pointing left from this one</summary>
	public Direction ToLeft {
		get {
			Direction result = new Direction(direction);
			result.direction |= 1;  //0001
			result.direction &= 13; //1101

			return result;
		}
	}

	///<summary>Returns a new Direction pointing left</summary>
	public static Direction Left {
		get {
			return None.ToLeft;
		}
	}

	///<summary>Returns a new Direction pointing right from this one</summary>
	public Direction ToRight {
		get {
			Direction result = new Direction(direction);
			result.direction |= 2;  //0010
			result.direction &= 14; //1110

			return result;
		}
	}

	///<summary>Returns a new Direction pointing right</summary>
	public static Direction Right {
		get {
			return None.ToRight;
		}
	}

	///<summary>Returns a new Direction pointing up from this one</summary>
	public Direction ToUp {
		get {
			Direction result = new Direction(direction);
			result.direction |= 4; //0100
			result.direction &= 7; //0111

			return result;
		}
	}

	///<summary>Returns a new Direction pointing up</summary>
	public static Direction Up {
		get {
			return None.ToUp;
		}
	}

	///<summary>Returns a new Direction pointing down from this one</summary>
	public Direction ToDown {
		get {
			Direction result = new Direction(direction);
			result.direction |= 8;  //1000
			result.direction &= 11; //1011

			return result;
		}
	}

	///<summary>Returns a new Direction pointing down</summary>
	public static Direction Down {
		get {
			return None.ToDown;
		}
	}

	/// <param name="d">Initial direction</param>
	/// <returns>Returns the opposite direction</returns>
	/// <remarks>Returns none if originally none</remarks>
	public static Direction operator ~(Direction d) {
		Direction result = None;

		if(d.IsLeft()) result = result.ToRight;
		if(d.IsRight()) result = result.ToLeft;
		if(d.IsUp()) result = result.ToDown;
		if(d.IsDown()) result = result.ToUp;

		return result;
	}

	public bool IsNone() {
		return direction == 0;
	}

	public bool IsLeft() {
		return (direction & 1) == 1;
	}

	public bool IsRight() {
		return (direction & 2) == 2;
	}

	public bool IsUp() {
		return (direction & 4) == 4;
	}

	public bool IsDown() {
		return (direction & 8) == 8;
	}

	public string Name() {
		string s = "";

		if(IsNone()) s = "none ";
		if(IsUp()) s += "up ";
		if(IsDown()) s += "down ";
		if(IsRight()) s += "left ";
		if(IsLeft()) s += "right ";

		return s;
	}
}