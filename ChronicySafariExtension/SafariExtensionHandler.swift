//
//  SafariExtensionHandler.swift
//  ChronicySafariExtension
//
//  Created by Alexandru Istrate on 12/03/2019.
//

import SafariServices;
import ChronicyFramework;

class SafariExtensionHandler: SFSafariExtensionHandler {
    
    override func messageReceived(withName messageName: String, from page: SFSafariPage, userInfo: [String : Any]?) {
        // This method will be called when a content script provided by your extension calls safari.extension.dispatchMessage("message").
        page.getPropertiesWithCompletionHandler { properties in
            NSLog("The extension received a message (\(messageName)) from a script injected into (\(String(describing: properties?.url))) with userInfo (\(userInfo ?? [:]))")
            
            guard let url: URL = properties?.url else {
                return;
            }
            
            ContentTrackerManager.manager.sendData(data: url, trackerType: .url);
        }
    }
    
    override func messageReceivedFromContainingApp(withName messageName: String, userInfo: [String : Any]? = nil) {
        NSLog("The extension received a message (\(messageName)) from containing app with userInfo (\(userInfo ?? [:]))");
    }
    
    override func validateToolbarItem(in window: SFSafariWindow, validationHandler: @escaping ((Bool, String) -> Void)) {
        // This is called when Safari's state changed in some way that would require the extension's toolbar item to be validated again.
        validationHandler(true, "");
    }
    
    override func popoverViewController() -> SFSafariExtensionViewController {
//        if let tasks: [String] = DistributedObjectManager.manager.get(for: SharedConstants.DistributedObjectKeys.tasks, action: .keepUnchanged) {
//            print(tasks);
//        }
        
        return SafariExtensionViewController.shared;
    }

}
