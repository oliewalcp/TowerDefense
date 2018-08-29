using System;
public class RoomMessage {
	private const uint number_bit = 0xFFFF0000;
	private const uint person_bit = 0xF000;
	private const uint difficulty_bit = 0xF00;
	private const uint checkpoint_bit = 0xFF;
	private uint message = 0;//房间信息

	private uint Get(uint num, uint _bit){
		return num & _bit;
	}
	public RoomMessage(uint number, uint person, uint difficulty, uint checkpoint) {
		message = 0;
		message |= Get(number << 16, number_bit) | Get(person << 12, person_bit) | Get(difficulty << 8, difficulty_bit) | Get(checkpoint, checkpoint_bit);
	}

	public RoomMessage(uint message) {
		this.message = message;
	}

	public static RoomMessage Parse(string msg) {
		if(msg.Length > 4) return null;
		return new RoomMessage(uint.Parse(msg));
	}

	public void SetPersonNumber(uint num) {
		message &= ~person_bit;
		message |= Get(num << 12, person_bit);
	}

	public uint GetPersonNumber() {
		uint result = message & person_bit;
		return result >> 12;
	}

	public void AddPerson() {
		SetPersonNumber(GetPersonNumber() + 1);
	}

	public void ReducePerson() {
		SetPersonNumber(GetPersonNumber() - 1);
	}

	public void SetRoomNumber(uint number) {
		message &= ~number_bit;
		message |= Get(number << 16, number_bit);
	}

	public ushort GetRoomNumber() {
		uint result = message & number_bit;
		return (ushort)(result >> 16);
	}

	public void SetDifficulty(uint difficulty) {
		message &= ~difficulty_bit;
		message |= Get(difficulty << 8, difficulty_bit);
	}

	public uint GetDifficulty() {
		uint result = message & difficulty_bit;
		return result >> 12;
	}

	public void SetCheckpoint(uint checkpoint) {
		message &= ~checkpoint_bit;
		message |= Get(checkpoint, checkpoint_bit);
	}

	public uint GetCheckpoint() {
		return message & checkpoint_bit;
	}

	public string GetString() {
		byte[] b = BitConverter.GetBytes(message);
		return BitConverter.ToString(b);
	}

	public override int GetHashCode(){
		return GetRoomNumber().GetHashCode();
	}
	public override bool Equals(object obj){
		RoomMessage rm = (RoomMessage)obj;
		return GetRoomNumber() == rm.GetRoomNumber();
	}
}
