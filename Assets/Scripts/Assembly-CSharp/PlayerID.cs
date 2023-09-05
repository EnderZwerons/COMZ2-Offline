public class PlayerID
{
	public AvatarType avatar_type = AvatarType.None;

	public AvatarData.AvatarState avatar_state = AvatarData.AvatarState.Normal;

	public string player_name = string.Empty;

	public int tnet_id = -1;

	public PlayerID(AvatarType type, AvatarData.AvatarState state, string name, int id)
	{
		avatar_type = type;
		avatar_state = state;
		player_name = name;
		tnet_id = id;
	}

	public override string ToString()
	{
		return "name:" + player_name + " tnet_id:" + tnet_id + " avatar_type:" + avatar_type;
	}
}
