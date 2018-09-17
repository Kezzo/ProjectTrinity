using System;

[Serializable]
public class Command {
		public string playerId;
		public string command;

		public int sequence;

		public static int commandSequence = 0;

		public Command (string playerId, string command, bool countUpSequence = true) {
			this.playerId = playerId;
			this.command = command;
			
			this.sequence = Command.commandSequence;

			if(countUpSequence) {
				Command.commandSequence++;
			}
		}
	}