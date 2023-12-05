namespace Ubik.Accounting.WebApp.Client.Components.Common.Forms
{
    //Dont' forget to include that in Tailwind safelist or class will not be generated
    //Will not use css constant anymore, with tailwind it's some kind of a pain... prefer different version of compo.
    //For this part, INVALID state i think we have a better solution with using tailwind state and change the state.... (one day)
    public static class UbikFormCSS
    {
        public const string LABEL_VALID_CSS_CLASS = "block mb-2 text-sm font-medium text-gray-900 dark:text-white";
        public const string LABEL_INVALID_CSS_CLASS = "block mb-2 text-sm font-medium text-red-700 dark:text-red-500";

        public const string SELECT_VALID_CSS_CLASS = "bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500";
        public const string SELECT_INVALID_CSS_CLASS = "bg-red-50 border border-red-500 text-red-900 placeholder-red-700 text-sm rounded-lg focus:ring-red-500 dark:bg-gray-700 focus:border-red-500 block w-full p-2.5 dark:text-red-500 dark:placeholder-red-500 dark:border-red-500";

        public const string TEXTINPUT_VALID_CSS_CLASS = "bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500";
        public const string TEXTINPUT_INVALID_CSS_CLASS = "bg-red-50 border border-red-500 text-red-900 placeholder-red-700 text-sm rounded-lg focus:ring-red-500 dark:bg-gray-700 focus:border-red-500 block w-full p-2.5 dark:text-red-500 dark:placeholder-red-500 dark:border-red-500";

        public const string TEXTAREA_VALID_CSS_CLASS = "block p-2.5 w-full text-sm text-gray-900 bg-gray-50 rounded-lg border border-gray-300 focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500";
        public const string TEXTAREA_INVALID_CSS_CLASS = "bg-red-50 border border-red-500 text-red-900 placeholder-red-700 text-sm rounded-lg focus:ring-red-500 dark:bg-gray-700 focus:border-red-500 block w-full p-2.5 dark:text-red-500 dark:placeholder-red-500 dark:border-red-500";
    }
}
