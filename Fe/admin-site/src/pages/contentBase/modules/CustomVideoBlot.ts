import Quill from 'quill';

const BlockEmbed = Quill.import('blots/block/embed');

class CustomVideo extends BlockEmbed {
  static blotName = 'video';
  static tagName = 'iframe';

  static create(value: any) {
    const node = super.create();
    node.setAttribute('frameborder', '0');
    node.setAttribute('allowfullscreen', 'true');
    
    if (typeof value === 'string') {
      node.setAttribute('src', value);
    } else {
      node.setAttribute('src', value.src);
      if (value.style) node.setAttribute('style', value.style);
    }

    return node;
  }

  static value(node: any) {
    return {
      src: node.getAttribute('src'),
      style: node.getAttribute('style')
    };
  }
}

Quill.register(CustomVideo);