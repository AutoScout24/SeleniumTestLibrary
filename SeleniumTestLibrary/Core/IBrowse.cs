using System.Collections.Generic;

namespace AutoScout24.SeleniumTestLibrary.Core
{
    interface IBrowse
    {
        void NavigateTo(string url, bool killPromptWindows = true);
        void Close();
        void DismissDialog();
        void ExecuteScript(string javascript);
        void ScrollIntoViewCss(string elementCss);
        void ScrollIntoViewId(string elementId);
        void SwitchTo(string windowId);

        string GetPageSource();
        int GetStatusCode(string url);
        void CaptureScreenShot(string prefferedFileName);
        string GetSelectedDropDownText(string dropDownId, int waitForSeconds = 0);
        string GetSelectedDropDownValue(string dropDownId, int waitForSeconds = 0);
        string GetDropDownText(string dropDownId, int waitForSeconds = 0);
        string GetElementValue(string elementId, int waitForSeconds = 0);

        IElement FindElementByCss(string css);
        IElement FindElementById(string id);
        IElement FindElementByName(string name);        
        IEnumerable<IElement> FindElementsWithCss(string css);                

        void WaitPageTitleContains(string title, int waitForSeconds = 30);        
        void WaitForDropDownValue(string value, int waitForSeconds = 30);
        void WaitForElementsInDropDown(string parentId, int noOfElements, int waitForSeconds = 30);
        void WaitForPageLoad(int waitForSeconds = 30);
        void WaitUrlContains(string value, int waitForSeconds = 30);
        void WaitForEvent(string eventName, int timeoutInSeconds = 30);
        void WaitForNoOfElements(string css, int noOfElements, int waitForSeconds = 30);

        IElement WaitForElementWithId(string id, int waitForSeconds = 30);        
        IElement WaitForElementWithName(string name, int waitForSeconds = 30);
        IElement WaitForElementWithCss(string css, int waitForSeconds = 30);

        IElement WaitElementWithIdIsVisible(string id, int waitForSeconds = 30);
        IElement WaitElementWithCssIsVisible(string css, int waitForSeconds = 30);        

        IElement MoveMouseOverElementWithId(string id, int waitForMiliseconds);

        bool ElementWithIdExists(string id);
        bool ElementWithNameExists(string name);
        bool ElementWithCssExists(string css);

        bool ElementWithIdIsVisible(string id);
        bool ElementWithNameIsVisible(string name);
        bool ElementWithCssIsVisible(string css);

        void AddCookie(string name, string value);
    }    
}

