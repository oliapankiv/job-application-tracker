export abstract class ReminderConfig {
   /**
	 * @methods GET
	 */
	public static readonly UPCOMING = '/reminders/upcoming';

   /**
	 * @methods PUT | DELETE
	 */
	public static readonly ITEM = (id: number): string => `/reminders/${id}`;

  /**
	 * @methods POST
	 */
	public static readonly CREATE = (id: number): string => `/applications/${id}/reminders`;
}
