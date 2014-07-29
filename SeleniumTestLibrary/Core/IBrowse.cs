using System;
using System.Collections.Generic;

using AutoScout24.SeleniumTestLibrary.Common;
using AutoScout24.SeleniumTestLibrary.Criteria;

namespace AutoScout24.SeleniumTestLibrary.Core
{
    public interface IBrowse
    {
        void NavigateTo(string url, bool killPromptWindows = true);
        void Close();
        void DismissDialog();        
        void ScrollIntoViewCss(string elementCss);
        void ScrollIntoViewId(string elementId);
        void SwitchTo(string windowId);

        string ExecuteScript(string javascript);
        string GetUrl();
        string GetPageSource();
        int GetStatusCode(string url);
        void CaptureScreenShot(string prefferedFileName);
        string GetSelectedDropDownText(string dropDownId, int waitForSeconds = 0);
        string GetSelectedDropDownValue(string dropDownId, int waitForSeconds = 0);
        string GetDropDownText(string dropDownId, int waitForSeconds = 0);
        string GetElementValue(string elementId, int waitForSeconds = 0);

        IElement Find(ICriteria criterion, bool onlyVisible = true);
        IEnumerable<IElement> FindAll(ICriteria criterion, bool onlyVisible = true);
        IElement WaitFor(ICriteria criterion, bool onlyVisible = true, int timeout = 30);

        [Obsolete("This method is deprecated, please use Find method instead.")]
        IElement FindElementByCss(string css);
        [Obsolete("This method is deprecated, please use Find method instead.")]
        IElement FindElementById(string id);
        [Obsolete("This method is deprecated, please use Find method instead.")]
        IElement FindElementByName(string name);
        [Obsolete("This method is deprecated, please use FindAll method instead.")]
        IEnumerable<IElement> FindElementsWithCss(string css);

        void RegisterWaitForEvent(string eventName);

        void WaitPageTitleContains(string title, int waitForSeconds = 30);        
        void WaitForDropDownValue(string value, int waitForSeconds = 30);
        void WaitForElementsInDropDown(string parentId, int noOfElements, int waitForSeconds = 30);
        void WaitForPageLoad(int waitForSeconds = 30);
        void WaitUrlContains(string value, int waitForSeconds = 30);
        void WaitForEvent(string eventName, int timeoutInSeconds = 30);
        void WaitForNoOfElements(string css, int noOfElements, int waitForSeconds = 30);

        [Obsolete("This method is deprecated, please use Find method instead.")]
        IElement WaitForElementWithId(string id, int waitForSeconds = 30);
        [Obsolete("This method is deprecated, please use Find method instead.")]
        IElement WaitForElementWithName(string name, int waitForSeconds = 30);
        [Obsolete("This method is deprecated, please use Find method instead.")]
        IElement WaitForElementWithCss(string css, int waitForSeconds = 30);

        [Obsolete("This method is deprecated, please use WaitFor method instead.")]
        IElement WaitElementWithIdIsVisible(string id, int waitForSeconds = 30);
        [Obsolete("This method is deprecated, please use Find method instead.")]
        IElement WaitElementWithCssIsVisible(string css, int waitForSeconds = 30);        

        IElement MoveMouseOverElementWithId(string id, int waitForMiliseconds);

        bool ElementWithIdExists(string id);
        bool ElementWithNameExists(string name);
        bool ElementWithCssExists(string css);

        bool ElementWithIdIsVisible(string id);
        bool ElementWithNameIsVisible(string name);
        bool ElementWithCssIsVisible(string css);

        void AddCookie(string name, string value);

        void SetDisplaySize(Device device);

        void EnableFeature(string featureBeeName);
        void DisableFeature(string featureBeeName);
        bool IsFeatureEnabled(string featureBeeName);
    }    
}

