//
//  CardTag.swift
//  ChronicyFrameworkMacOS
//
//  Created by Alexandru Istrate on 11/04/2019.
//

import Foundation;
import CoreGraphics;

public struct CardTag: Equatable {
    public var title: String;
    public var description: String?;
    public var color: CGColor;
    
    public init(title: String, description: String? = nil, color: CGColor) {
        self.title = title;
        self.description = description;
        self.color = color;
    }
}

public class CardTagManager {
    
    public private(set) var tags: [CardTag] = [];
    
    public init() {
        self.setupDefaults();
    }
    
    public func register(tag: CardTag) {
        self.tags.append(tag);
    }
    
    public func deregister(tag: CardTag) {
        self.tags.removeAll { (iter: CardTag) -> Bool in
            return iter == tag;
        }
    }
    
    public func tags(matchingColor: CGColor) -> [CardTag] {
        return self.tags.filter({ (tag: CardTag) -> Bool in
            return tag.color == matchingColor;
        });
    }
}

extension CardTagManager {
    private func setupDefaults() {
//        register(tag: CardTag(title: "Tag 1", color: CGColor(red: 1.0, green: 0.0, blue: 0.0, alpha: 1.0)));
    }
}
