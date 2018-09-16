public class Command {
		public string playerId;
		public string command;

		public int counter;

		public static int commandCounter = 0;

		public Command (string playerId, string command) {
			this.playerId = playerId;
			this.command = command;
			this.counter = Command.commandCounter;
			Command.commandCounter++;
		}
	}