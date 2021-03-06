//
//  SharedConstants.swift
//  ChronicyFrameworkMacOS
//
//  Created by Alexandru Istrate on 18/03/2019.
//

import Foundation;

public class SharedConstants {
    public static var appGroupSuiteName: String = "extension.ro.internals";
    public static var defaultApplicationsPlistName: String = "DefaultApplications";
    
    public class DistributedObjectKeys {
        public static var pageURLData: String = NSLocalizedString("currentPageURL", comment: "");
        
        public static var notebooks: String = "notebooks";
        public static var stacks: String = "tasks";
        
        public static var browserSelectedNotebook: String = "browserSelectedNotebook";
        public static var browserSelectedStack: String = "browserSelectedTask";
    }
}
